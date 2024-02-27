using System;
using System.Collections.Generic;

namespace UniFramework.Localization
{
    public class TableData
    {
        // 数据集合
        private readonly Dictionary<string, object> _datas = null;

        public TableData(int capacity)
        {
            _datas = new Dictionary<string, object>(capacity);
        }
        public TableData(Dictionary<string, object> datas)
        {
            _datas = datas;
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        public void AddValue(string key, object value)
        {
            _datas.Add(key, value);
        }

        /// <summary>
        /// 获取元素
        /// </summary>
        public object GetValue(string key)
        {
            if (_datas.TryGetValue(key, out object value))
            {
                return value;
            }
            else
            {
                UniLogger.Warning($"Not found key value : {key}");
                return null;
            }
        }

        /// <summary>
        /// 尝试获取元素
        /// </summary>
        public object TryGetValue(string key)
        {
            if (_datas.TryGetValue(key, out object value))
            {
                return value;
            }
            else
            {
                return null;
            }
        }
    }
}