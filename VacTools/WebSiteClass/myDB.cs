using System;
using System.Collections.Generic;
using System.Web;
using System.Collections;
using System.Web.UI.WebControls;
using System.Data.SqlClient;
using System.Data;
using System.Web.Configuration;
using System.Text.RegularExpressions;
using System.Text;
using aiet.Tools;
using MySql.Data.MySqlClient;
using Aiet_DB;

/// <summary>
/// myDB 的摘要描述
/// throwException : default=false，發生錯誤時處理， 當 true 時拋出錯誤訊息，
///                  當 false 時， 執行 select 傳回 null、 執行 Insert/Delete/Update 傳回 -1
/// htmlEncode : default=true，自動將SQL 內容有 < 、> 的符號，自動轉換成 &gt、&lt 避免XSS
/// errorMessage : 取得錯誤訊息
/// logError : 記錄錯誤訊息
/// </summary>
public class myDB : baseDB
{
    public tools tool = new tools();
    public logError clsError = new logError();
    public Boolean isMulti { get; set; }
    public Boolean DoLogerr { set; get; }

    /// <summary>
    /// 讀取圖表的資料庫名稱
    /// </summary>
    public string ChartsDBN
    {
        get
        {
            string tmpdb = WebConfigurationManager.ConnectionStrings["tmpdb"].ConnectionString;
            Regex reg = new Regex(@"(Catalog|DataBase)=([^;]+)");
            return reg.Match(tmpdb).Groups[2].Value;
        }
    }

    public string ConnectionString
    {
        get
        {
            return WebConfigurationManager.ConnectionStrings[connectString].ConnectionString;
        }
    }

    public myDB()
    {
        DoLogerr = true;
        //
        // TODO: 在此加入建構函式的程式碼
        //
        connectString = "sqlcn"; //--指定 web.config 的變數名稱
        qry.ProviderName = "MySql.Data.MySqlClient";
    }

    public myDB(string sqlcn)
    {
        connectString = sqlcn;
        //this.throwException = true;
        //this.htmlEncode = true;
    }

    #region 存放查詢結果，加快chart處理速度

    public void tmpdb(GridView gv, string tablename)
    {
        string column = "";
        foreach (DataControlField df in gv.Columns)
        {
            if (df.SortExpression != "")
            {
                if (df.GetType().Name == "BoundField")
                {
                    BoundField bf = (BoundField)df;
                    if (bf.DataFormatString == "{0:N1}")
                    {
                        column = string.Format(" {0}[{1}] [numeric](18,1) NULL,", column, df.SortExpression);
                        continue;
                    }
                    else if (bf.DataFormatString == "{0:0.0}")
                    {
                        column = string.Format(" {0}[{1}] [numeric](18,1) NULL,", column, df.SortExpression);
                        continue;
                    }
                    else if (bf.DataFormatString == "{0:N0}")
                    {
                        column = string.Format(" {0}[{1}] [int] NULL,", column, df.SortExpression);
                        continue;
                    }
                    else if (bf.DataFormatString == "{0:0}")
                    {
                        column = string.Format(" {0}[{1}] [int] NULL,", column, df.SortExpression);
                        continue;
                    }
                }
                column = string.Format(" {0}[{1}] [nvarchar](400) NULL,", column, df.SortExpression);
            }
        }

        this.selectText = string.Format(@"use [{2}]
IF OBJECT_ID('[dbo].[{0}]') IS NOT NULL
BEGIN
DROP TABLE [dbo].[{0}]
END

CREATE TABLE [dbo].[{0}](
	[aID] [nvarchar](50) NOT NULL,
    {1}
 CONSTRAINT [PK_{0}] PRIMARY KEY CLUSTERED
(
	[aID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]  ", tablename, column, ChartsDBN);

        this.selectSQL();
    }

    #endregion 存放查詢結果，加快chart處理速度

    public MySqlCommand storeProcedure(string name)
    {
        MySqlConnection sqlcnn = new MySqlConnection();
        sqlcnn.ConnectionString = new myDB().connectString;
        MySqlCommand sqlcmd = new MySqlCommand();
        sqlcmd.Connection = sqlcnn;
        sqlcmd.CommandText = name;
        sqlcmd.CommandType = CommandType.StoredProcedure;

        return sqlcmd;
    }

    public int GetMaxID(string tbname)
    {
        return Convert.ToInt32(this.selectSQL("select max(id) from " + tbname)[0][0]) + 1;
    }

    #region How to get autoincrement Identity value

    public string currentTableIdentity(string tablename)
    {
        ListItemCollection pp = new ListItemCollection();
        //pp.Add(new ListItem("tblname", tablename));
        //System.Data.DataView dv = this.selectSQL("SELECT IDENT_CURRENT(@tblname)", pp);
        System.Data.DataView dv = this.selectSQL("SELECT max(aid) from " + tablename, pp);
        if (dv != null && dv.Count > 0) { return dv[0][0].ToString(); } else { return "0"; }
    }

    public string nextTableIdentity(string tablename)
    {
        ListItemCollection pp = new ListItemCollection();
        pp.Add(new ListItem("tblname", tablename));
        System.Data.DataView dv = this.selectSQL("SELECT IDENT_CURRENT(@tblname) + IDENT_INCR(@tblname)", pp);
        if (dv != null && dv.Count > 0) { return dv[0][0].ToString(); } else { return "1"; }
    }

    #endregion How to get autoincrement Identity value

    //-- 實作SQL發生錯誤時寫入的記錄
    public override void logError(string errMsg)
    {
        if (DoLogerr)
        {
            new logError(new object[] { -1, errMsg });
        }
    }

    public Tuple<string, List<ListItem>> CombinWhereIn(string paraName, params string[] paras)
    {
        int i = 1;
        var items = new List<ListItem>();
        StringBuilder sb = new StringBuilder();
        foreach (var p in paras)
        {
            sb.AppendFormat("@{0}{1},", paraName, i);
            items.Add(new ListItem(string.Concat(paraName, i), p));

            i++;
        }

        return Tuple.Create(sb.ToString().TrimEnd(','), items);
    }


}