using System;
using System.Collections.Generic;

namespace UniFramework.Localization
{
    public class TableCollection
    {
        // 数据表集合
        // 说明：Key为地区文化编码
        private readonly Dictionary<string, TableData> _tables = new Dictionary<string, TableData>(1000);

        /// <summary>
        /// 数据表名称
        /// </summary>
        public string TableName { protected set; get; }

        /// <summary>
        /// 获取表格数据
        /// </summary>
        public TableData GetTableData(string cultureCode)
        {
            if (_tables.ContainsKey(cultureCode) == false)
            {
                UniLogger.Error($"Not found table data : {cultureCode}");
                return null;
            }
            return _tables[cultureCode];
        }

        /// <summary>
        /// 添加表格数据
        /// </summary>
        public void AddTableData(string cultureCode, TableData tableData)
        {
            if (_tables.ContainsKey(cultureCode) == false)
            {
                UniLogger.Warning($"The data table already exists : {cultureCode}");
                return;
            }
            _tables.Add(cultureCode, tableData);
        }
    }
}