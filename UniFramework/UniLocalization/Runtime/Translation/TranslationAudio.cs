using UnityEngine;

namespace UniFramework.Localization
{
    public class TranslationAudio : ITranslation
    {
        public object GetTranslationResult(LocalizedBehaviour bhv)
        {
            return GetTranslationResult(bhv.DataTableName, bhv.TranslationKey);
        }

        public object GetTranslationResult(string dataTableName, string translationKey)
        {
            var dataValue = UniLocalization.GetLocalizeValue(dataTableName, translationKey);
            if (dataValue != null)
            {
                bool correntValue = dataValue is AudioClip;
                if (correntValue == false)
                {
                    UniLogger.Warning($"The data table value type is not {nameof(AudioClip)} : {dataTableName}");
                    return null;
                }
            }
            return dataValue;
        }
    }
}