using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VacWebSiteTools.ODT;
using System.Data;
using Aiet_DB;

namespace VacWebSiteTools.Helper
{
    /// <summary>
    /// 資料庫資料表欄位資訊
    /// </summary>
    public class TableColmunsHelper : ITableColmunsHelper
    {
        private baseDB _db;

        public TableColmunsHelper()
        {
            _db = RepositoryFactory.GetMyDB();
        }

        public TableColmunsHelper(baseDB db)
        {
            _db = db;
        }

        public List<TableColmuns> SelectableColumnsName(string tableName)
        {
            var dv = _db.SelectTableColumnsInfo(tableName);
            return dv.ToTable().AsEnumerable().Select(p => new TableColmuns
            {
                ColumnName = p["COLUMN_NAME"].ToString(),
                ColumnType = p["COLUMN_TYPE"].ToString(),
                IsNullable = p["IS_NULLABLE"].ToString().ToUpper() == "YES",
            }).ToList();
        }

        /// <summary>
        /// 將資料表的欄位組合成給Sql 用的字串 , 
        /// 回傳一個Tuple  Items1是欄位名稱 , Items2是@欄位名稱
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="removeColumns"></param>
        /// <returns>回傳一個Tuple  Items1是欄位名稱 , Items2是@欄位名稱</returns>
        public Tuple<List<string> , List<string>> GetTableColumnsForCommand(string tableName , List<string> removeColumns = null)
        {
            var colmns = SelectableColumnsName(tableName);
            if (removeColumns != null)
                colmns.RemoveAll(p => removeColumns.Contains(p.ColumnName));

            var r1 = colmns.Select(p => p.ColumnName).ToList();
            var r2 = colmns.Select(p => string.Concat("@",p.ColumnName)).ToList();
            return Tuple.Create(r1, r2);
        }
            
    }

}