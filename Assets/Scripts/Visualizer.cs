using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using MediaPipe.BlazeFace;
using System.Collections;

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    //[SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField, Range(0, 1)] float _threshold = 0.75f;
    [SerializeField] RawImage _previewUI = null;
    [SerializeField] Marker _markerPrefab = null;

    #endregion

    #region Private members

    FaceDetector _detector;
    Marker[] _markers = new Marker[16];
    private bool isReady = false;
    private RenderTexture _gpuTexture;

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        StartCoroutine(WaitForCameraTexture());
        
    }

    IEnumerator WaitForCameraTexture()
    {
        while (!SharedCameraManager.IsInitialized)
        {
            Debug.Log("Waiting for SharedCameraManager to initialize...");
            yield return null;
        }

        Debug.Log("SharedCameraManager initialized. Proceeding with Visualizer setup.");
        // Face detector initialization
        _detector = new FaceDetector(_resources);

        // Marker population
        for (var i = 0; i < _markers.Length; i++)
            _markers[i] = Instantiate(_markerPrefab, _previewUI.transform);

        var sharedTexture = SharedCameraManager.CameraTexture;
        _gpuTexture = new RenderTexture(256, 256, 0);

        isReady = true;
    }

    void OnDestroy()
    {
        _detector?.Dispose();
        if (_gpuTexture != null)
        {
            _gpuTexture.Release();
            _gpuTexture = null;
        }
    }

    void LateUpdate()
    {
        if (!isReady) return;

        var sharedTexture = SharedCameraManager.CameraTexture;

        if (sharedTexture == null)
        {
            Debug.LogWarning("SharedCameraManager has no valid texture.");
            return;
        }
        Graphics.Blit(sharedTexture, _gpuTexture);

            _detector.ProcessImage(_gpuTexture, _threshold);

            // Marker 업데이트
            var i = 0;
            foreach (var detection in _detector.Detections)
            {
                if (i == _markers.Length) break;
                var marker = _markers[i++];
                marker.detection = detection;
                marker.gameObject.SetActive(true);
            }

            for (; i < _markers.Length; i++)
                _markers[i].gameObject.SetActive(false);

            // UI 업데이트
            _previewUI.texture = _gpuTexture;

        
        /*
        // Face detection
        _detector.ProcessImage(_source.Texture, _threshold);

        // Marker update
        var i = 0;

        foreach (var detection in _detector.Detections)
        {
            if (i == _markers.Length) break;
            var marker = _markers[i++];
            marker.detection = detection;
            marker.gameObject.SetActive(true);
        }

        for (; i < _markers.Length; i++)
            _markers[i].gameObject.SetActive(false);

        // UI update
        _previewUI.texture = _source.Texture;*/
    }

    #endregion
}