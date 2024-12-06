using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Android;

public class SharedCameraManager : MonoBehaviour
{
    private static SharedCameraManager _instance;
    private WebCamTexture _webCamTexture;
    private bool _isInitialized = false;

    public static bool IsInitialized => _instance != null && _instance._isInitialized;
    public static Texture CameraTexture
    {
        get
        {
            if (_instance == null)
            {
                Debug.LogWarning("SharedCameraManager instance is null.");
                return null;
            }

            if (_instance._webCamTexture == null)
            {
                Debug.LogWarning("WebCamTexture is null in SharedCameraManager.");
                return null;
            }

            return _instance._webCamTexture;
        }
    }

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
                Debug.Log("Waiting for camera permission...");
                yield return null;
            }
        }

        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            foreach (var device in devices)
            {
                if (device.isFrontFacing)
                {
                    _webCamTexture = new WebCamTexture(device.name);
                    break;
                }
            }

            if (_webCamTexture == null)
            {
                Debug.LogWarning("No front-facing camera found. Using default camera.");
                _webCamTexture = new WebCamTexture(devices[0].name);
            }

            _webCamTexture.Play();
            
            // 대기: WebCamTexture가 실행될 때까지
            while (!_webCamTexture.didUpdateThisFrame)
            {
                Debug.Log("Waiting for WebCamTexture to start...");
                yield return null;
            }

            Debug.Log("WebCamTexture initialized successfully.");
            _isInitialized = true;
        }
        else
        {
            Debug.LogError("No webcam detected.");
        }
    }

}