using System;
using System.Globalization;
using System.Collections;
using System.Collections.Generic;

namespace UniFramework.Localization
{
    public static class UniLocalization
    {
        private static bool _isInitialize = false;
        private static readonly List<LocaleIdentifier> _locales = new List<LocaleIdentifier>(100);
        private static readonly Dictionary<System.Type, ITranslation> _translations = new Dictionary<System.Type, ITranslation>(100);
        private static readonly Dictionary<string, TableCollection> _tableCollections = new Dictionary<string, TableCollection>(100);
        private static LocaleIdentifier _currentLocale = null;


        /// <summary>
        /// 当本地化地区发生改变
        /// </summary>
        public static event System.Action OnLocalizationChanged;

        /// <summary>
        /// 初始化本地化系统
        /// </summary>
        public static void Initalize(List<LocaleIdentifier> locales)
        {
            if (_isInitialize)
                throw new Exception($"{nameof(UniLocalization)} is initialized !");

            if (_isInitialize == false)
            {
                if (locales.Count == 0)
                    throw new Exception($"The param locales list cannot be empty!");

                _locales.AddRange(locales);
                _currentLocale = _locales[0];

                // 创建驱动器
                _isInitialize = true;
                UniLogger.Log($"{nameof(UniLocalization)} initalize !");
            }
        }

        /// <summary>
        /// 销毁本地化系统
        /// </summary>
        public static void Destroy()
        {
            if (_isInitialize)
            {

            }
        }

        /// <summary>
        /// 改变本地化地区
        /// </summary>
        public static void ChangeLocale(string cultureCode)
        {
            foreach (var locale in _locales)
            {
                if (locale.Code == cultureCode)
                {
                    _currentLocale = locale;
                    OnLocalizationChanged.Invoke();
                    return;
                }
            }

            UniLogger.Error($"Not found locale : {cultureCode}");
        }

        /// <summary>
        /// 获取当前地区的文化信息
        /// </summary>
        public static CultureInfo GetCurrentCulture()
        {
            return _currentLocale.Culture;
        }

        /// <summary>
        /// 获取翻译器的实例
        /// </summary>
        public static ITranslation GetOrCreateTranslation(System.Type translationType)
        {
            if (translationType == null)
                return null;

            if (_translations.TryGetValue(translationType, out var translation))
            {
                return translation;
            }
            else
            {
                translation = (ITranslation)Activator.CreateInstance(translationType);
                if (translation == null)
                    throw new Exception($"Failed careate {nameof(ITranslation)} instance : {translationType.FullName}");
                _translations.Add(translationType, translation);
                return translation;
            }
        }

        /// <summary>
        /// 添加数据集合
        /// </summary>
        public static void AddTableCollection(TableCollection collection)
        {
            _tableCollections.Add(collection.TableName, collection);
        }

        /// <summary>
        /// 获取数据集合
        /// </summary>
        public static TableCollection GetTableCollection(string tableName)
        {
            if (_tableCollections.ContainsKey(tableName) == false)
            {
                UniLogger.Error($"Not found table collection : {tableName}");
                return null;
            }

            return _tableCollections[tableName];
        }

        /// <summary>
        /// 获取数据
        /// </summary>
        public static object GetTableDataValue(string tableName, string translationKey)
        {
            var tableData = _tableCollections[tableName].GetTableData(_currentLocale.Code);
            return tableData.GetValue(translationKey);
        }
    }
}