using UnityEngine;

namespace UniFramework.Localization
{
    [ExecuteInEditMode]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(AudioSource))]
    [LocalizedBehaviour(typeof(TranslationAudio))]
    public class LocalizedAudio : LocalizedBehaviour
    {
        private AudioSource _audioSource;

        protected override void OnTranslation(ITranslation translation)
        {
            if (_audioSource == null)
                _audioSource = GetComponent<AudioSource>();

            _audioSource.clip = translation.GetTranslationResult(this) as AudioClip;
        }
    }
}