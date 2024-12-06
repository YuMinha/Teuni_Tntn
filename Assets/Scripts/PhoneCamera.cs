using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Pool;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    [SerializeField] private Visualizer _visualizer;
    [SerializeField] private FoodNearHandler _foodNearHandler;
    private bool isCamera;
    private WebCamTexture cameraTexture;
    private Texture bckgDefault;
    //public GameObject rects;

    private ObjectPool<GameObject> boxPool;

    List<Color> colorTag = new List<Color>();

    public RawImage bckg;
    public AspectRatioFitter fit;
    public Yolov5Detector yolov5Detector;
    public GameObject boxContainer;
    public GameObject boxPrefab;
    public GameObject background;
    public TextMeshProUGUI text;

    private int NETWORK_SIZE_X;
    private int NETWORK_SIZE_Y;


    private int framesCount = 0;
    private float timeCount = 0.0f;
    private float refreshTime = 1.0f;
    public float fps = 0.0f;

    public GameObject LoadingPanel; //로딩 화면
    public GameObject StartPanel; //시작 화면
    private bool isLoadingComplete = false; // 로딩 완료 상태 확인

    void Start()
    {
        NETWORK_SIZE_X = GameObject.Find("Detector").GetComponent<Yolov5Detector>().GetNewtorkX();
        NETWORK_SIZE_Y = GameObject.Find("Detector").GetComponent<Yolov5Detector>().GetNewtorkY();
        int CLASS_COUNT = GameObject.Find("Detector").GetComponent<Yolov5Detector>().GetClassCount();

        for (int i = 0; i < CLASS_COUNT; i++)
        {
            Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.4f);
            colorTag.Add(randomColor);
        }

        boxPool = new ObjectPool<GameObject>(
            createFunc: CreateBoxObject,
            actionOnGet: ActivateBox,
            actionOnRelease: DeactivateBox,
            collectionCheck: false,
            defaultCapacity: 10,
            maxSize: 50
        );

        bckgDefault = bckg.texture;

        StartCoroutine(WaitForSharedCamera());
    }

    GameObject CreateBoxObject()
    {
        GameObject newBox = Instantiate(boxPrefab, boxContainer.transform);
        newBox.SetActive(false);
        
        return newBox;
    }

    void ActivateBox(GameObject box)
    {
        if (box == null)
        {
            Debug.LogError("Attempted to activate a null GameObject!");
            return;
        }
        box.SetActive(true);
    }

    void DeactivateBox(GameObject box)
    {
        if (box == null)
        {
            Debug.LogError("Attempted to deactivate a null GameObject!");
            return;
        }
        box.SetActive(false);
        TextMeshProUGUI label = box.GetComponentInChildren<TextMeshProUGUI>();
        if (label != null)
        {
            label.text = string.Empty; // 텍스트 초기화
        }
    }


    IEnumerator WaitForSharedCamera()
    {
        while (!SharedCameraManager.IsInitialized)
        {
            Debug.Log("Waiting for SharedCameraManager to initialize...");
            yield return null;
        }

        Debug.Log("SharedCameraManager initialized.");
        isCamera = true;

        var sharedTexture = SharedCameraManager.CameraTexture;
        if (sharedTexture != null)
        {
            bckg.texture = sharedTexture;
            float ratio_ = ((RectTransform)background.transform).rect.width / 1080;
            boxContainer.transform.localScale = new Vector2(ratio_, ratio_);

            WebCamTexture webCamTexture = (WebCamTexture)sharedTexture;
            int orient = -webCamTexture.videoRotationAngle;
            bckg.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

            ResizeRectTransform();
        }
    }

    void ResizeRectTransform()
    {
        float cameraAspect = 3f / 4f;
        RectTransform rectTransform = bckg.rectTransform;
        float screenWidth = Screen.width;

        fit.aspectRatio = cameraAspect;
        rectTransform.sizeDelta = new Vector2(screenWidth, screenWidth / cameraAspect);
    }


    /*TODO
     * 디텍션, 박스 초기화, 라벨 작업 다 따로 병렬화? 시키기
     * Box UI 풀링 방식으로 바꾸기
     */
    void Update()
    {
        if (!isCamera)
        {
            Debug.LogWarning("isCamera is false. Skipping Update.");
            return;
        }

        var sharedTexture = SharedCameraManager.CameraTexture;

        if (sharedTexture == null)
        {
            Debug.LogWarning("SharedCameraManager returned null for CameraTexture.");
            return;
        }

        if (!(sharedTexture is WebCamTexture webCamTexture))
        {
            Debug.LogError("CameraTexture is not a WebCamTexture.");
            return;
        }

        if (!webCamTexture.isPlaying)
        {
            Debug.LogWarning("WebCamTexture is not running.");
            return;
        }

        //Debug.Log("WebCamTexture is ready and playing. Proceeding with detection.");

        StartCoroutine(WaitForDetection(webCamTexture.GetPixels32(), webCamTexture.width));
    }
    IEnumerator WaitForDetection(Color32[] pixels, int width)
    {
        yield return yolov5Detector.Detect(pixels, width, boxes =>
        {
            if (isLoadingComplete == false)
            {
                Debug.Log("디텍트 시작");
                LoadingPanel.SetActive(false);//로딩화면 비활성화
                StartPanel.SetActive(true);//시작화면 활성화
                isLoadingComplete = true;
            }
            ProcessDetectionResults(boxes);
        });
    }

    void ProcessDetectionResults(IList<BoundingBox> detectedBoxes)
    {
        foreach (Transform child in boxContainer.transform)
        {
            if (child != null && child.gameObject != null)
            {
                boxPool.Release(child.gameObject);
            }
            else
            {
                Debug.LogWarning("Attempted to release a destroyed object.");
            }
        }

        CheckFoodNearMouth(detectedBoxes);

        StartCoroutine(UpdateUIBoxes(detectedBoxes));
    }

    IEnumerator UpdateUIBoxes(IList<BoundingBox> boxes)
    {
        foreach (var box in boxes)
        {
            GameObject newBox = boxPool.Get();
            if (newBox == null)
            {
                Debug.LogWarning("ObjectPool returned a null GameObject!");
                continue;
            }

            newBox.name = box.Label + " " + box.Confidence;
            newBox.GetComponent<Image>().color = colorTag[box.LabelIdx];

            newBox.transform.localPosition = new Vector3(box.Rect.x - NETWORK_SIZE_X / 2, box.Rect.y - NETWORK_SIZE_Y / 2);
            newBox.transform.localScale = new Vector2(box.Rect.width / 100, box.Rect.height / 100);

            TextMeshProUGUI label = newBox.GetComponentInChildren<TextMeshProUGUI>();
            if (label != null)
            {
                label.text = $"{box.Label}\n{box.Confidence:F3}";
            }
            else
            {
                Debug.LogWarning("Label TextMeshProUGUI is missing in the pooled object!");
            }
            yield return null;
        }
    }

    private void CountFps()
    {
        if (timeCount < refreshTime)
        {
            timeCount += Time.deltaTime;
            framesCount += 1;
        }
        else
        {
            fps = (float)framesCount / timeCount;
            // Debug.Log("FPS: " + fps);
            text.text = "FPS: " + fps;
            framesCount = 0;
            timeCount = 0.0f;
        }
    }

    bool IsFoodNearMouth(Rect foodBox, Rect mouthArea)
    {
        var container = (RectTransform)boxContainer.transform;
        Vector2 containerSize = container.sizeDelta;

        //정규화 된 좌표였음
        Rect mouthAReaInPixel = new Rect(
            mouthArea.x * containerSize.x,
            mouthArea.y * containerSize.y,
            mouthArea.width * containerSize.x,
            mouthArea.height * containerSize.y);

        Vector2 foodCenter = new Vector2(
            foodBox.x + foodBox.width / 2,
            foodBox.y + foodBox.height / 2);

        Debug.Log($"Food Center: {foodCenter}, Food Box: {foodBox}");
        Debug.Log($"Mouth Area: {mouthAReaInPixel}");


        return mouthAReaInPixel.Contains(foodCenter);
    }

    void CheckFoodNearMouth(IList<BoundingBox> foodDetections)
    {
        if (_visualizer == null || _visualizer.MouthArea == Rect.zero)
        {
            Debug.Log("입 인식 오류");
            return;
        }

        Rect mouthArea = _visualizer.MouthArea;

        foreach (var food in foodDetections)
        {
            if (IsFoodNearMouth(food.Rect, mouthArea))
            {
                Debug.Log($"{food.Label} 입 주변에 있음!!");
                _foodNearHandler.EatFoodToGetCoins(food.Label);
            }
        }
    }
}
