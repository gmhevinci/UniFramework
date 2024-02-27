using UnityEngine;
using UnityEngine.UI;

namespace UniFramework.Localization
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [LocalizedBehaviour(typeof(TranslationSprite))]
    public class LocalizedImage : LocalizedBehaviour
    {
        private Image _image;

        protected override void OnTranslation(ITranslation translation)
        {
            if (_image == null)
                _image = GetComponent<Image>();

            _image.sprite = translation.GetTranslationResult(this) as Sprite;
        }
    }
}