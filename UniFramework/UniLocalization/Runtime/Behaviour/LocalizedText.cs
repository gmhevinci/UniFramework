using UnityEngine;
using UnityEngine.UI;

namespace UniFramework.Localization
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Text))]
    [LocalizedBehaviour(typeof(TranslationString))]
    public class LocalizedText : LocalizedBehaviour
    {
        private Text _text;

        protected override void OnTranslation(ITranslation translation)
        {
            if (_text == null)
                _text = GetComponent<Text>();

            _text.text = (string)translation.GetTranslationResult(this);
        }
    }
}