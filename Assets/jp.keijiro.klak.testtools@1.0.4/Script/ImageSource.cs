using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Klak.TestTools {

public sealed class ImageSource : MonoBehaviour
{
    #region Public property

    public Texture Texture => OutputBuffer;
    public Vector2Int OutputResolution => _outputResolution;

    #endregion

    #region Editable attributes

    // Source type options
    public enum SourceType { Texture, Video, Webcam, Card, Gradient }
    [SerializeField] SourceType _sourceType = SourceType.Card;

    // Webcam options
    [SerializeField] string _webcamName = "";
    [SerializeField] Vector2Int _webcamResolution = new Vector2Int(1080, 1440);
    [SerializeField] int _webcamFrameRate = 30;

    // Output options
    [SerializeField] RenderTexture _outputTexture = null;
    [SerializeField] Vector2Int _outputResolution = new Vector2Int(1920, 1080);

    #endregion

    #region Package asset reference

    [SerializeField, HideInInspector] Shader _shader = null;

    #endregion

    #region Private members

    UnityWebRequest _webTexture;
    WebCamTexture _webcam;
    Material _material;
    RenderTexture _buffer;

    RenderTexture OutputBuffer
      => _outputTexture != null ? _outputTexture : _buffer;

    // Blit a texture into the output buffer with aspect ratio compensation.
    void Blit(Texture source, bool vflip = false)
    {
        if (source == null) return;

        var aspect1 = (float)source.width / source.height;
        var aspect2 = (float)OutputBuffer.width / OutputBuffer.height;

        var scale = new Vector2(aspect2 / aspect1, aspect1 / aspect2);
        scale = Vector2.Min(Vector2.one, scale);
        if (vflip) scale.y *= -1;

        var offset = (Vector2.one - scale) / 2;

        Graphics.Blit(source, OutputBuffer, scale, offset);
    }

    #endregion

    #region MonoBehaviour implementation

    void Start()
    {
        // Allocate a render texture if no output texture has been given.
        if (_outputTexture == null)
            _buffer = new RenderTexture
              (_outputResolution.x, _outputResolution.y, 0);

        // Webcam source type:
        // Create a WebCamTexture and start capturing.
        if (_sourceType == SourceType.Webcam)
        {
            _webcam = new WebCamTexture
              (_webcamName,
               _webcamResolution.x, _webcamResolution.y, _webcamFrameRate);
            _webcam.Play();
        }

    }

    void OnDestroy()
    {
        if (_webcam != null) Destroy(_webcam);
        if (_buffer != null) Destroy(_buffer);
        if (_material != null) Destroy(_material);
    }

    void Update()
    {
        if (_sourceType == SourceType.Webcam && _webcam.didUpdateThisFrame)
            Blit(_webcam, _webcam.videoVerticallyMirrored);

        // Asynchronous image downloading
        if (_webTexture != null && _webTexture.isDone)
        {
            var texture = DownloadHandlerTexture.GetContent(_webTexture);
            _webTexture.Dispose();
            _webTexture = null;
            Blit(texture);
            Destroy(texture);
        }

    }

    #endregion
}

} // namespace Klak.TestTools
