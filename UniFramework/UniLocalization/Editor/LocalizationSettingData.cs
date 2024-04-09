using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    public class LocalizationSettingData
    {
        static LocalizationSettingData()
        {
        }

        private static LocalizationSetting _setting = null;
        public static LocalizationSetting Setting
        {
            get
            {
                if (_setting == null)
                {
                    _setting = YooAsset.Editor.SettingLoader.LoadSettingData<LocalizationSetting>();
                    for(int i=0; i<_setting.TableSettings.Count; i++)
                    {
                        var tableSetting = _setting.TableSettings[i];
                        tableSetting.LoadSetting();
                    }
                }
                return _setting;
            }
        }

        public static ILocalizeReader GetLocalizeReader(string tableName)
        {
            foreach(var tableSetting in Setting.TableSettings)
            {
                if (tableSetting.TableName == tableName)
                {
                    if (tableSetting.ReaderInstance == null)
                        Debug.LogWarning($"The table reader instance is invalid : {tableName}");
                    return tableSetting.ReaderInstance;
                }
            }

            Debug.LogWarning($"Not found localize table setting: {tableName}");
            return null;
        }
    }
}