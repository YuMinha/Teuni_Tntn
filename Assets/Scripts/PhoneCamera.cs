using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PhoneCamera : MonoBehaviour
{
    private bool isCamera = false;
    private WebCamTexture cameraTexture;
    private Texture bckgDefault;
    private static Texture2D boxOutlineTexture;
    public GameObject rects;

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

    void Start()
    {
        // Yolov5Detector 관련 설정
        NETWORK_SIZE_X = GameObject.Find("Detector").GetComponent<Yolov5Detector>().GetNewtorkX();
        NETWORK_SIZE_Y = GameObject.Find("Detector").GetComponent<Yolov5Detector>().GetNewtorkY();
        int CLASS_COUNT = GameObject.Find("Detector").GetComponent<Yolov5Detector>().GetClassCount();

        for (int i = 0; i < CLASS_COUNT; i++)
        {
            Color randomColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), 0.4f);
            colorTag.Add(randomColor);
        }

        StartCoroutine(WaitForCameraTexture());
    }

    IEnumerator WaitForCameraTexture()
    {
        while(!SharedCameraManager.IsInitialized)
        {
            yield return null;
        }

        Debug.Log("CameraTexture 초기화");

        InitWithCameraTexture();
    }

    void InitWithCameraTexture()
    {
        // SharedCameraManager의 CameraTexture 활용
        var sharedTexture = SharedCameraManager.CameraTexture;

        if (sharedTexture == null)
        {
            Debug.LogError("CameraTexture is not initialized in SharedCameraManager.");
            isCamera = false;
            return;
        }

        // UI에 Texture 연결
        bckg.texture = sharedTexture;
        Debug.Log("백그라운드에 shared 연결");

        // 비율 조정 및 화면 설정
        float ratio_ = ((RectTransform)background.transform).rect.width / CAMERA_CAPTURE_X;
        boxContainer.transform.localScale = new Vector2(ratio_, ratio_);

        

        float ratio = 4f / 3f;
        fit.aspectRatio = ratio;

        // 카메라 방향 설정
        int orient = -((WebCamTexture)sharedTexture).videoRotationAngle;
        bckg.rectTransform.localEulerAngles = new Vector3(0, 0, orient);

        ResizeRectTransform();
        isCamera = true;
    }

    void ResizeRectTransform()
    {
        // RectTransform 가져오기
        RectTransform rectTransform = bckg.rectTransform;

        // 디바이스 화면 가로 길이
        float screenWidth = Screen.width;

        // 카메라 비율 계산 (가로/세로)
        float cameraAspect = 4f / 3f;

        // 가로 길이를 디바이스의 가로 길이에 맞춤
        rectTransform.sizeDelta = new Vector2(screenWidth, screenWidth / cameraAspect);
    }

// Update is called once per frame
void Update()
    {
        if (!isCamera)
            return;

        /*Texture texture = bckg.texture;

        WebCamTexture newTexture = (WebCamTexture)bckg.texture;
        StartCoroutine(yolov5Detector.Detect(newTexture.GetPixels32(), newTexture.width, boxes =>
        {
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
        }));*/

        var sharedTexture = SharedCameraManager.CameraTexture;

        if (sharedTexture == null || !(sharedTexture is WebCamTexture webCamTexture))
        {
            Debug.LogWarning("Shared Camera Texture is not available or not a WebCamTexture.");
            return;
        }

        if (webCamTexture.isPlaying)
        {
            StartCoroutine(yolov5Detector.Detect(webCamTexture.GetPixels32(), webCamTexture.width, boxes =>
            {
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
        }
        else
        {
            Debug.LogWarning("WebCamTexture is not playing.");
        }

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