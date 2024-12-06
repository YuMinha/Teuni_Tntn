using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool isCamera;
    private WebCamTexture cameraTexture;
    private Texture bckgDefault;
    private static Texture2D boxOutlineTexture;
    //public GameObject rects;

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

    public int CAMERA_CAPTURE_X = 1080;
    public int CAMERA_CAPTURE_Y = 1440;

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

        bckgDefault = bckg.texture;

        StartCoroutine(WaitForSharedCamera());
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

            ResizeRectTransform();
        }
    }

        void ResizeRectTransform()
    {
        RectTransform rectTransform = bckg.rectTransform;
        float screenWidth = Screen.width;
        float cameraAspect = 4f / 3f;
        rectTransform.sizeDelta = new Vector2(screenWidth, screenWidth / cameraAspect);
    }

// Update is called once per frame
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
        int orient = -webCamTexture.videoRotationAngle;
        bckg.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        if (!webCamTexture.isPlaying)
    {
        Debug.LogWarning("WebCamTexture is not running.");
        return;
    }

    //Debug.Log("WebCamTexture is ready and playing. Proceeding with detection.");

        StartCoroutine(yolov5Detector.Detect(webCamTexture.GetPixels32(), webCamTexture.width, boxes =>
        {
            if(isLoadingComplete == false)
            {
                LoadingPanel.SetActive(false);//로딩화면 비활성화
                StartPanel.SetActive(true);//시작화면 활성화
            }
            

            isLoadingComplete = true;

            Resources.UnloadUnusedAssets();


            foreach (Transform child in boxContainer.transform)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < boxes.Count; i++)
            {
                GameObject newBox = Instantiate(boxPrefab);
                newBox.name = boxes[i].Label + " " + boxes[i].Confidence;
                newBox.GetComponent<Image>().color = colorTag[boxes[i].LabelIdx];
                newBox.transform.parent = boxContainer.transform;
                newBox.transform.localPosition = new Vector3(boxes[i].Rect.x - NETWORK_SIZE_X / 2, boxes[i].Rect.y - NETWORK_SIZE_Y / 2);
                newBox.transform.localScale = new Vector2(boxes[i].Rect.width / 100, boxes[i].Rect.height / 100);

                bool text = true;
                if (text)
                {
                    GameObject labelText = new GameObject("LabelText");
                    labelText.transform.parent = newBox.transform;
                    labelText.transform.localPosition = Vector3.zero;
                    Text label = labelText.AddComponent<Text>();
                    label.text = boxes[i].Label + " " + boxes[i].Confidence.ToString("F3");
                    label.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
                    label.fontSize = 30;
                    label.color = Color.white;
                    label.alignment = TextAnchor.MiddleCenter;

                    RectTransform labelRect = label.GetComponent<RectTransform>();
                    Vector2 pivotOffset = new Vector2(0.5f, 0.5f) - newBox.GetComponent<RectTransform>().pivot;
                    labelRect.localPosition = pivotOffset * newBox.GetComponent<RectTransform>().sizeDelta;
                }
            }
        }));

        CountFps();

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
}