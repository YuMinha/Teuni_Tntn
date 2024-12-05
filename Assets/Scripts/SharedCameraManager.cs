using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharedCameraManager : MonoBehaviour
{
    private static SharedCameraManager _instance;
    private WebCamTexture _webCamTexture;

    public static Texture CameraTexture => _instance._webCamTexture;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        InitializeCamera();
    }

    private void InitializeCamera()
    {
        WebCamDevice[] devices = WebCamTexture.devices;

        if (devices.Length > 0)
        {
            _webCamTexture = new WebCamTexture(devices[0].name);
            _webCamTexture.Play();
        }
        else
        {
            Debug.LogError("No webcam detected.");
        }
    }

    public static Texture2D GetTexture2D()
    {
        if (_instance._webCamTexture == null) return null;

        Texture2D texture = new Texture2D(_instance._webCamTexture.width, _instance._webCamTexture.height, TextureFormat.RGB24, false);
        texture.SetPixels32(_instance._webCamTexture.GetPixels32());
        texture.Apply();
        return texture;
    }
}
