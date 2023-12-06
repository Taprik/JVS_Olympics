using UnityEngine;
using UnityEngine.UI;

namespace GlobalOutline
{
    public class OutlineCanvas : MonoBehaviour
    {
        private Canvas _canvas;
        private RawImage _rawImage;
        private int _width;
        private int _height;

        private RenderTexture _tempRenderTexture;

        private void Awake()
        {
            if (gameObject.GetComponent<Canvas>() == null)
                _canvas = gameObject.AddComponent<Canvas>();
            else 
                _canvas = gameObject.GetComponent<Canvas>();

            if (_canvas != null) _canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            if (_canvas != null) _canvas.sortingOrder = 16960;

            if(gameObject.GetComponent<Graphic>() == null) _rawImage = gameObject.AddComponent<RawImage>();
            if(_rawImage != null) _rawImage.raycastTarget = false;
        }

        private void OnDestroy()
        {
            if (_tempRenderTexture != null)
            {
                RenderTexture.ReleaseTemporary(_tempRenderTexture);
            }
        }

        private void Update()
        {
            if (_width != Screen.width || _height != Screen.height)
            {
                if (_tempRenderTexture != null)
                {
                    RenderTexture.ReleaseTemporary(_tempRenderTexture);
                }
                _width = Screen.width;
                _height = Screen.height;
                _tempRenderTexture = RenderTexture.GetTemporary(_width, _height, 0);
                if (_rawImage != null) _rawImage.texture = _tempRenderTexture;
            }
            OutlineManager.Instance.FillTexture(_tempRenderTexture);
        }
    }
}
