
namespace UniFramework.Localization
{
    public interface ITranslation
    {
        /// <summary>
        /// 获取翻译结果
        /// </summary>
        object GetTranslationResult(LocalizedBehaviour bhv);

        /// <summary>
        /// 获取翻译结果
        /// </summary>
        object GetTranslationResult(string dataTableName, string translationKey);
    }
}