using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Data.SqlClient;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;
using aiet.Tools;
using MySql.Data.MySqlClient;

/// <summary>
/// logError 的摘要描述
/// 不要繼承任何 DB ，避免因SQL錯誤產生迴圈
/// </summary>
/// 
public class logError
{
    public string url = "";
    public string ip = new tools().getClientIP;
    public string source = "";

    public bool ResponseError { set; get; }
    string lineNumber(string s)
    {
        if (s == null) { return ""; }

        string pattern = @"([^\\])+\s\d+";
        System.Text.RegularExpressions.Regex reg = new System.Text.RegularExpressions.Regex(pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase | System.Text.RegularExpressions.RegexOptions.Multiline);
        System.Text.RegularExpressions.MatchCollection mc = reg.Matches(s);
        StringBuilder sb = new StringBuilder();

        if (mc == null) { return s; }

        for (int i = 0; i < mc.Count; i++)
        {
            sb.AppendFormat("{0};", mc[i].Value);
        }
        return sb.ToString().TrimEnd(';');
    }

    #region 寫入文字檔案
    public void writeFile(int errCode, Exception ex, string sql, string param, string errMsg = "")
    {
        string path = new aiet.Tools.tools().GetAppSeting("log");
        if (path == "") return;
        if (VirtualPathUtility.IsAppRelative(path))
        {
            path = HttpContext.Current.Server.MapPath(path);
        }

        // HttpContext.Current.Response.Write(path);

        path = string.Format(@"{0}\{1}", path, DateTime.Now.ToString("yyyyMM"));
        if (!Directory.Exists(path))
        {
            Directory.CreateDirectory(path);
        }

        string file = DateTime.Now.ToString("yyyyMMdd");

        file = string.Format(@"{0}\{1}.log", path, file);
        StreamWriter sw = null;
        try
        {
            sw = new StreamWriter(file, true);
            string Msg = (ex == null ? errMsg : ex.Message);
            string LineNo = (ex == null ? lineNumber(errMsg) : lineNumber(ex.StackTrace));
            if (sql == "")
            {
                file = string.Format(@"{0:yyyy/MM/dd HH:mm:ss} | Message:{1} |occured at {2} ", DateTime.Now, Msg, LineNo);
            }
            else
            {
                file = string.Format(@"{0:yyyy/MM/dd HH:mm:ss} | Message:{1} |occured at {2} | SQL:{3} | Parameter:{4}", DateTime.Now, Msg, LineNo, sql, param);
            }
        }
        finally
        {
            sw.WriteLine(file);
            sw.Close();
            sw.Dispose();
        }
    }
    #endregion


    #region 寫入資料
    public logError()
    {
        ResponseError = true;
    }

    public logError(string ErrMsg)
    {
        ResponseError = true;

        Write(new object[] { ErrMsg }); //-- 底層 SQL 發生錯誤
    }

    /// <summary>
    /// params : object[] {int errCode, Exception ex, string sql, string param,ListItemCollection pp}
    /// </summary>
    /// <param name="?"></param>
    public logError(object[] parameters)
    {
        ResponseError = true;

        Write(parameters);
    }

    public void Write(object[] parameters)
    {
        MySqlConnection SqlCnt = null;
        int errCode = 999; //--例外發生
        Exception ex = null;
        string sql = "";
        string param = "";
        foreach (object o in parameters)
        {
            if (o is int)
            {
                errCode = (int)o;
            }
            else if (o is Exception)
            {
                ex = (Exception)o;
            }
            else if (o is string)
            {
                if (sql == "") { sql = (string)o; } else { param = (string)o; }
            }
            else if (o is ListItemCollection)
            {
                foreach (KeyValuePair<string, string> k in (ListItemCollection)o)
                {
                    param += string.Format("{0}{1}={2}", param, k.Key, k.Value);
                }
                param = param.Trim(',');
            }

        }
        try
        {
            #region 設計時，打開訊息，可由 web.config 控制
            string errMsg = "";

            if (ex == null) // sql 底層錯誤代碼 : -1
            {
                errMsg = sql;
                url = HttpContext.Current == null ? "" : HttpContext.Current.Request.Path;
                ip = new tools().getClientIP;
            }
            else
            {
                errMsg = string.Format("Message:{0} | occured at {1} | SQL:{2} | Parameter:{3}", ex.ToString(), lineNumber(ex.StackTrace), sql, param);
            }

            if (ResponseError && System.Web.Configuration.WebConfigurationManager.AppSettings["design"] != null && System.Web.Configuration.WebConfigurationManager.AppSettings["design"] == "1")
            {
                if (HttpContext.Current != null)
                {
                    errMsg = string.Format("<script type='text/javascript'>alert('發生錯誤，訊息將顯示於網頁頂端'); </script><div style='background-color:#ffffff;color:red;'> 錯誤訊息:{0}</div>", errMsg);
                    HttpContext.Current.Response.Write(errMsg);
                    //return;
                }
            }
            #endregion


            //-- 此處不可以直接用 myDB 的method，需另外的 SqlCommand； 否則會無窮迴圈

            SqlCnt = new MySqlConnection(System.Web.Configuration.WebConfigurationManager.ConnectionStrings["sqlcn"].ConnectionString);
            SqlCnt.Open();
            var cmd = new MySqlCommand();
            cmd.Connection = SqlCnt;
            cmd.CommandText = "INSERT INTO LogError(ErrorTime,ErrorCode,ErrorMsg,url,ip,SourceID) VALUES(now(),@errCode,@errMsg,@url,@ip,@source)";
            cmd.Parameters.AddWithValue("errCode", errCode);
            cmd.Parameters.AddWithValue("errMsg", errMsg);
            cmd.Parameters.AddWithValue("url", url);
            cmd.Parameters.AddWithValue("ip", ip);
            cmd.Parameters.AddWithValue("source", source);
            cmd.ExecuteNonQuery();
            cmd.CommandText = "SELECT max(aid) from LogError";
            MySqlDataReader Reader = cmd.ExecuteReader();
            if (Reader.HasRows)
            {
                while (Reader.Read())
                {
                    errCode = Convert.ToInt32(Reader.GetValue(0));
                }
            }
        
            cmd.Dispose();
            SqlCnt.Close();
            if (new tools().GetAppSeting("design") == "0")
            {

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.AppendFormat("<html><head><title></title><meta http-equiv='Content-Type' content='text/html; charset=utf-8' /></head><body></body></html><script type='text/javascript'>alert('Code:{0} {1}'); window.history.go(-1); </script>", errCode, "數據錯誤");
                if (HttpContext.Current != null)
                {
                    HttpContext.Current.Response.Write(sb.ToString());
                    HttpContext.Current.Server.ClearError();
                }
            }
        }
        catch (Exception exx)
        {
            if (exx != null)
            {
                if (HttpContext.Current != null)
                {
                    new logError().writeFile(-1, exx, HttpContext.Current.Request.Path, "");

                    if (new tools().GetAppSeting("design") == "0")
                    {
                        System.Text.StringBuilder sb = new System.Text.StringBuilder();
                        sb.AppendFormat("<script type='text/javascript'>alert('Code: -2 {0}'); window.history.go(-1); </script>", "數據錯誤");
                        HttpContext.Current.Response.Write(sb.ToString());
                        HttpContext.Current.Server.ClearError();
                    }
                    else
                        throw;

                }

            }

        }
        finally
        {
            if (SqlCnt != null && SqlCnt.State == System.Data.ConnectionState.Open)
                SqlCnt.Close();
        }

    }
    #endregion

}


