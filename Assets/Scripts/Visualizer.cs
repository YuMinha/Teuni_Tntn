using UnityEngine;
using UnityEngine.UI;
using Klak.TestTools;
using MediaPipe.BlazeFace;

public sealed class Visualizer : MonoBehaviour
{
    #region Editable attributes

    [SerializeField] ImageSource _source = null;
    [SerializeField] ResourceSet _resources = null;
    [SerializeField, Range(0, 1)] float _threshold = 0.75f;
    [SerializeField] RawImage _previewUI = null;
    //[SerializeField] Marker _markerPrefab = null;
    [SerializeField] float foodDetectRange = 0.5f; // 얼굴 크기 기준 비율

    public Rect MouthArea { get; private set; }

    #endregion

    #region Private members

    FaceDetector _detector;
    Marker[] _markers = new Marker[16];
    float _timeSinceLastUpdate = 0;
    const float UpdateInterval = 0.1f; // 0.1초 간격

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Face detector initialization
        _detector = new FaceDetector(_resources);

        // Marker population
        //for (var i = 0; i < _markers.Length; i++)
        //    _markers[i] = Instantiate(_markerPrefab, _previewUI.transform);
    }

    void OnDestroy()
      => _detector?.Dispose();

    void LateUpdate()
    {
        _timeSinceLastUpdate += Time.deltaTime;
        if (_timeSinceLastUpdate >= UpdateInterval)
        {
            _timeSinceLastUpdate = 0;
            // Face detection
            _detector.ProcessImage(_source.Texture, _threshold);
            UpdateMarkers();
            UpdateMouthArea();
        }
        

        // UI update
        _previewUI.texture = _source.Texture;
    }

    void UpdateMarkers()
    {
        //// Marker update
        //var i = 0;

        //foreach (var detection in _detector.Detections)
        //{
        //    if (i == _markers.Length) break;
        //    var marker = _markers[i++];
        //    marker.detection = detection;
        //    marker.gameObject.SetActive(true);
        //}

        //for (; i < _markers.Length; i++)
        //    _markers[i].gameObject.SetActive(false);
    }

    void UpdateMouthArea()
    {
        if (_detector.Detections.Length > 0)
        {
            var detection = _detector.Detections[0];
            Vector2 mouthCenter = detection.mouth;
            float mouthRadius = detection.extent.x * foodDetectRange;
            MouthArea = new Rect(
                mouthCenter.x - mouthRadius,
                mouthCenter.y - mouthRadius,
                mouthRadius * 2,
                mouthRadius * 2
            );
        }
        else
        {
            MouthArea = Rect.zero;
        }
    }
    #endregion
}