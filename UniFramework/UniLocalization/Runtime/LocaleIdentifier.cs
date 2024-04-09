using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace UniFramework.Localization
{
    public class LocaleIdentifier
    {
        /// <summary>
        /// 文化信息类
        /// </summary>
        public CultureInfo Culture { get; private set; }
        
        /// <summary>
        /// 文化唯一编码
        /// </summary>
        public string CultureCode { get; private set; }

        public LocaleIdentifier(SystemLanguage systemLanguage)
        {
            CultureCode = GetSystemLanguageCultureCode(systemLanguage);
            Culture = CultureInfo.GetCultureInfo(CultureCode);
        }

        public static string GetSystemLanguageCultureCode(SystemLanguage lang)
        {
            switch (lang)
            {
                case SystemLanguage.Afrikaans: return "af";
                case SystemLanguage.Arabic: return "ar";
                case SystemLanguage.Basque: return "eu";
                case SystemLanguage.Belarusian: return "be";
                case SystemLanguage.Bulgarian: return "bg";
                case SystemLanguage.Catalan: return "ca";
                case SystemLanguage.Chinese: return "zh-CN";
                case SystemLanguage.ChineseSimplified: return "zh-hans";
                case SystemLanguage.ChineseTraditional: return "zh-hant";
                case SystemLanguage.SerboCroatian: return "hr";
                case SystemLanguage.Czech: return "cs";
                case SystemLanguage.Danish: return "da";
                case SystemLanguage.Dutch: return "nl";
                case SystemLanguage.English: return "en";
                case SystemLanguage.Estonian: return "et";
                case SystemLanguage.Faroese: return "fo";
                case SystemLanguage.Finnish: return "fi";
                case SystemLanguage.French: return "fr";
                case SystemLanguage.German: return "de";
                case SystemLanguage.Greek: return "el";
                case SystemLanguage.Hebrew: return "he";
                case SystemLanguage.Hungarian: return "hu";
                case SystemLanguage.Icelandic: return "is";
                case SystemLanguage.Indonesian: return "id";
                case SystemLanguage.Italian: return "it";
                case SystemLanguage.Japanese: return "ja";
                case SystemLanguage.Korean: return "ko";
                case SystemLanguage.Latvian: return "lv";
                case SystemLanguage.Lithuanian: return "lt";
                case SystemLanguage.Norwegian: return "no";
                case SystemLanguage.Polish: return "pl";
                case SystemLanguage.Portuguese: return "pt";
                case SystemLanguage.Romanian: return "ro";
                case SystemLanguage.Russian: return "ru";
                case SystemLanguage.Slovak: return "sk";
                case SystemLanguage.Slovenian: return "sl";
                case SystemLanguage.Spanish: return "es";
                case SystemLanguage.Swedish: return "sv";
                case SystemLanguage.Thai: return "th";
                case SystemLanguage.Turkish: return "tr";
                case SystemLanguage.Ukrainian: return "uk";
                case SystemLanguage.Vietnamese: return "vi";
#if UNITY_2022_2_OR_NEWER
                case SystemLanguage.Hindi: return "hi";
#endif
                default: return "";
            }
        }
    }
}