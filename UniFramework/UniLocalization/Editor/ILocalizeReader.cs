using System;
using System.Collections.Generic;

namespace UniFramework.Localization.Editor
{
    public interface ILocalizeReader
    {
        /// <summary>
        /// 本地化KEY集合
        /// </summary>
        List<string> Keys { get; }

        /// <summary>
        /// 读取数据
        /// </summary>
        void Read(string assetPath);
    }
}