using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.Android;

public class SharedCameraManager : MonoBehaviour
{
    private static SharedCameraManager _instance;
    private WebCamTexture _webCamTexture;
    private bool _isInitialized = false;

    public static bool IsInitialized => _instance != null && _instance._isInitialized;
    public static Texture CameraTexture => _instance?._webCamTexture;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        StartCoroutine(InitializeCamera());
    }

    IEnumerator InitializeCamera()
    {
        if (!Permission.HasUserAuthorizedPermission(Permission.Camera))
        {
            Permission.RequestUserPermission(Permission.Camera);
            while (!Permission.HasUserAuthorizedPermission(Permission.Camera))
            {
                yield return null;
            }
        }

        WebCamDevice[] devices = WebCamTexture.devices;
        if (devices.Length > 0)
        {
            // 전면 카메라 찾기
            foreach (var device in devices)
            {
                if (device.isFrontFacing)
                {
                    _webCamTexture = new WebCamTexture(device.name);
                    break;
                }
            }

            // 전면 카메라가 없으면 첫 번째 카메라 사용
            if (_webCamTexture == null)
            {
                Debug.LogWarning("No front-facing camera found. Using default camera.");
                _webCamTexture = new WebCamTexture(devices[0].name);
            }

            _webCamTexture.Play();

            // WebCamTexture가 초기화될 때까지 대기
            while (!_webCamTexture.didUpdateThisFrame)
            {
                yield return null;
            }

            Debug.Log("WebCamTexture initialized successfully with front-facing camera.");
            _isInitialized = true; // 초기화 완료 플래그 설정
        }
        else
        {
            Debug.LogError("No webcam detected.");
        }
    }
}

