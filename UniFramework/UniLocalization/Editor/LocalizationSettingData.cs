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
                    _setting = YooAsset.Editor.SettingLoader.LoadSettingData<LocalizationSetting>();
                return _setting;
            }
        }
    }
}