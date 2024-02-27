
namespace UniFramework.Localization
{
    public class TranslationString : ITranslation
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
                bool correntValue = dataValue is string;
                if (correntValue == false)
                    UniLogger.Warning($"The data table value type is not String : {dataTableName}");
            }
            return dataValue;
        }
        public object GetTranslationResult(string dataTableName, string translationKey, params object[] args)
        {
            var dataValue = UniLocalization.GetTableDataValue(dataTableName, translationKey);
            if (dataValue != null)
            {
                bool correntValue = dataValue is string;
                if (correntValue == false)
                    UniLogger.Warning($"The data table value type is not String : {dataTableName}");
            }

            var culture = UniLocalization.GetCurrentCulture();
            return StringFormatter.FormateString((string)dataValue, culture, args);
        }
    }
}