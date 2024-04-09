using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    [CreateAssetMenu(fileName = "LocalizationSetting", menuName = "UniFramework/Create Localization Setting")]
    public class LocalizationSetting : ScriptableObject
    {
        [Serializable]
        public class TableSetting
        {
            public string TableName;
            public string LocalizeReaderClassName;
            public UnityEngine.Object FileObject;

            [NonSerialized]
            public ILocalizeReader ReaderInstance;

            public void LoadSetting()
            {
                if (string.IsNullOrEmpty(TableName))
                    return;
                if (string.IsNullOrEmpty(LocalizeReaderClassName))
                    return;
                if (FileObject == null)
                    return;

                // 获取配置路径
                string assetPath = AssetDatabase.GetAssetPath(FileObject);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogWarning($"Not found localize table data file : {assetPath}");
                    return;
                }

                // 获取读取器类型
                TypeCache.TypeCollection collection = TypeCache.GetTypesDerivedFrom<ILocalizeReader>();
                System.Type readerType = null;
                foreach (var type in collection)
                {
                    if (type.Name == LocalizeReaderClassName)
                    {
                        readerType = type;
                        break;
                    }
                }
                if (readerType == null)
                {
                    Debug.LogWarning($"Not found {nameof(ILocalizeReader)} type with name {LocalizeReaderClassName}");
                    return;
                }

                // 实例化读取器并解析数据
                ReaderInstance = (ILocalizeReader)Activator.CreateInstance(readerType);
                ReaderInstance.Read(assetPath);
            }
        }

        public List<TableSetting> TableSettings = new List<TableSetting>();
    }
}