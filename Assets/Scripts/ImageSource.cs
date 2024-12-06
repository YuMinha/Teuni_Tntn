using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Video;

namespace Klak.TestTools
{

    public sealed class ImageSource : MonoBehaviour
    {
        #region Public property

        public Texture Texture => OutputBuffer;
        public Vector2Int OutputResolution => _outputResolution;

        #endregion

        #region Editable attributes
        // Output options
        [SerializeField] RenderTexture _outputTexture = null;
        [SerializeField] Vector2Int _outputResolution = new Vector2Int(256, 256);

        #endregion

        #region Private members

        UnityWebRequest _webTexture;
        RenderTexture _buffer;
        bool _isInit = false;

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
            //_webcam = new WebCamTexture
            //  (_webcamName,
            //   _webcamResolution.x, _webcamResolution.y, _webcamFrameRate);
            //_webcam.Play();

            StartCoroutine(WaitForSharedCamera());
        }

        System.Collections.IEnumerator WaitForSharedCamera()
        {
            while (!SharedCameraManager.IsInitialized)
            {
                Debug.Log("Waiting for SharedCam in Face");
                yield return null;
            }

            var sharedTexture = SharedCameraManager.CameraTexture;
            if (sharedTexture == null)
            {
                Debug.LogError("SharedCameraManager returned null for CameraTexture.");
                yield break;
            }

            if (!(sharedTexture is WebCamTexture))
            {
                Debug.LogError("SharedCameraManager returned an unsupported texture type.");
                yield break;
            }

            Debug.Log("SharedCameraManager initialized and CameraTexture is ready.");
            _isInit = true;
        }

        void OnDisable()
        {
            var sharedTexture = SharedCameraManager.CameraTexture as WebCamTexture;
            if (sharedTexture != null)
            {
                sharedTexture.Stop();
            }
        }


        void OnDestroy()
        {
            if (_buffer != null)
            {
                _buffer.Release();
                Destroy(_buffer);
            }

            if (_outputTexture != null)
            {
                _outputTexture.Release();
                Destroy(_outputTexture);
            }
        }

        void Update()
        {
            if (!_isInit) return;

            var sharedTexture = SharedCameraManager.CameraTexture as WebCamTexture;
            if (sharedTexture == null)
            {
                Debug.LogWarning("SharedCameraManager texture is not available or not a WebCamTexture.");
                return;
            }

            if (!sharedTexture.isPlaying)
            {
                Debug.LogWarning("WebCamTexture is not running. Ensure it is started properly.");
                return;
            }
            Blit(sharedTexture);


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