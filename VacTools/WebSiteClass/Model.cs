using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
/// <summary>
/// Model 的摘要描述
/// </summary>
using System.Data;
using System.Web.UI.WebControls;



public class DataModel
{
    public DataTable Data { set; get; }

    /// <summary>
    /// 總筆數
    /// </summary>
    public int Total { set; get; }

    /// <summary>
    /// 資料已讀取完
    /// </summary>
    public bool End { set; get; }
}

