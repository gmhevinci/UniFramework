using System;
using System.Collections.Generic;

namespace UniFramework.Localization
{
    public class TableData
    {
        private readonly Dictionary<string, object> _datas = null;

        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { private set; get; }

        /// <summary>
        /// 文化编码
        /// </summary>
        public string CultureCode { private set; get; }


        public TableData(string tableName, string cultureCode, int capacity)
        {
            TableName = tableName;
            CultureCode = cultureCode;
            _datas = new Dictionary<string, object>(capacity);
        }
        public TableData(string tableName, string cultureCode, Dictionary<string, object> datas)
        {
            TableName = tableName;
            CultureCode = cultureCode;
            _datas = datas;
        }

        /// <summary>
        /// 添加元素
        /// </summary>
        public void AddValue(string key, object value)
        {
            if (_datas.ContainsKey(key))
            {
                UniLogger.Warning($"The key already existed : {key}");
                return;
            }
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