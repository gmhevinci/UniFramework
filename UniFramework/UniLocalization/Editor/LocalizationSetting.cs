using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace UniFramework.Localization.Editor
{
    [CreateAssetMenu(fileName = "LocalizationSetting", menuName = "UniFramework/Create Localization Setting")]
    public class LocalizationSetting : ScriptableObject
    {
        public List<string> TableNames = new List<string>();
    }
}