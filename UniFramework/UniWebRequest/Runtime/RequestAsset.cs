using System.Diagnostics;
using UnityEngine;

namespace UniFramework.WebRequest
{
    public class RequestAsset
    {
        private UnityEngine.Object _assetObject { set; get; }
        private Sprite _cacheSprite;

        /// <summary>
        /// 资源对象的URL地址
        /// </summary>
        public string AssetURL { private set; get; }


        internal RequestAsset(string assetURL, UnityEngine.Object assetObject)
        {
            AssetURL = assetURL;
            _assetObject = assetObject;
        }

        /// <summary>
        /// 获取精灵对象
        /// </summary>
        public Sprite GetSprite()
        {
            if (_cacheSprite == null)
            {
                var texture = GetTexture();
                if (texture != null)
                    _cacheSprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
            }
            return _cacheSprite;
        }

        /// <summary>
        /// 获取纹理对象
        /// </summary>
        public Texture2D GetTexture()
        {
            if (_assetObject == null)
                return null;

            if (_assetObject is Texture2D)
            {
                return _assetObject as Texture2D;
            }
            else
            {
                UniLogger.Warning($"Request asset type is not {nameof(Texture2D)}");
                return null;
            }
        }

        /// <summary>
        /// 获取音频对象
        /// </summary>
        public AudioClip GetAudioClip()
        {
            if (_assetObject == null)
                return null;

            if (_assetObject is AudioClip)
            {
                return _assetObject as AudioClip;
            }
            else
            {
                UniLogger.Warning($"Request asset type is not {nameof(AudioClip)}");
                return null;
            }
        }

        /// <summary>
        /// 卸载资源对象
        /// </summary>
        public void UnloadAsset()
        {
            if (_assetObject != null)
            {
                Resources.UnloadAsset(_assetObject);
                _assetObject = null;
            }
        }
    }
}