using UnityEngine;

namespace UniFramework.Localization
{
    public class TranslationSprite : ITranslation
    {
        public object GetTranslationResult(LocalizedBehaviour bhv)
        {
            return GetTranslationResult(bhv.DataTableName, bhv.TranslationKey);
        }

        public object GetTranslationResult(string dataTableName, string translationKey)
        {
            var dataValue = UniLocalization.GetTableDataValue(dataTableName, translationKey);
            if (dataValue != null)
            {
                bool correntValue = dataValue is Sprite;
                if (correntValue == false)
                    UniLogger.Warning($"The data table value type is not Sprite : {dataTableName}");
            }
            return dataValue;
        }
    }
}