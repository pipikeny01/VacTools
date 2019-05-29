using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Collections;
using System.Data;
using aiet.Base;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;


/// <summary>
/// clsAccount 的摘要描述
/// </summary>
public class clsParameters : BaseTable, IclsParameters
{
    private TeacherQualityualifiODT _teacherQualityualifiODT;

    public virtual TeacherQualityualifiODT TeacherQualityualifi
    {
        get
        {
            return _teacherQualityualifiODT ?? GeTeacherQualityualifi();
        }
    }

    /// <summary>
    /// 教師排課資格ODT
    /// </summary>
    public class TeacherQualityualifiODT
    {
        /// <summary>
        /// 年齡限制
        /// </summary>
        public int Age_limit { set; get; }

        /// <summary>
        /// 內聘時數限制
        /// </summary>
        public int Iteacher_hour_limit { set; get; }

        /// <summary>
        /// 外聘時數限制
        /// </summary>
        public int Oteacher_hour_limit { set; get; }
    }


    public clsParameters()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
        this.TableName = "Parameter";
        NameField = "Pname";
        dfs = new DataFields("Pname", "Value");
    }

    /// <summary>
    /// 取出多個參數值
    /// </summary>
    /// <param name="Pname">參數名稱</param>
    /// <returns></returns>
    public Dictionary<string, string> GetSysParamValue(params string[] Pname)
    {
        _params.Clear();

        var PnameTuple = this.CombinWhereIn("Pname", Pname);
        _params.AddRange(PnameTuple.Item2.ToArray());
        DataView dv = this.selectSQL(string.Format(@"
SELECT * FROM {0}
WHERE Pname in {1} order by Sort", this.TableName, PnameTuple.Item1), _params);
        Dictionary<string, string> dic = new Dictionary<string, string>();
        if (dv != null)
        {
            for (int i = 0; i < dv.Count; i++)
            {
                dic.Add(dv[i]["Value"].ToString(), dv[i]["Text"].ToString());
            }
        }
        return dic;
    }

    /// <summary>
    /// 取出單一參數值
    /// </summary>
    /// <param name="Pname">參數名稱</param>
    /// <returns></returns>
    public string ParamValue(string Pname)
    {
        _params.Clear();
        _params.Add(new ListItem("Pname", Pname));

        DataView dv = this.selectSQL(string.Format("SELECT * FROM {0} WHERE Pname=@Pname order by Sort", this.TableName), _params);
        return dv.Count == 0 ? "" : dv[0]["Value"].ToString();
    }

    public bool isExisted(string Pname, string value)
    {
        _params.Clear();
        _params.Add(new ListItem("Pname", Pname));
        _params.Add(new ListItem("Value", value));

        DataView dv = this.selectSQL(string.Format("SELECT * FROM {0} WHERE Pname=@Pname and Value=@Value", this.TableName), _params);
        return dv.Count > 0 ? true : false;
    }

    public bool isExisted(string id, string Pname, string value)
    {
        _params.Clear();
        _params.Add(new ListItem("Pname", Pname));
        _params.Add(new ListItem("Value", value));
        _params.Add(new ListItem("ID", id));

        DataView dv = this.selectSQL(string.Format("SELECT * FROM {0} WHERE Pname=@Pname and Value=@Value and aID <> @ID", this.TableName), _params);
        return dv.Count > 0 ? true : false;
    }

    public DataView GroupParam(string GrpID)
    {
        _params.Clear();
        _params.Add(new ListItem("GrpID", GrpID));
        return this.selectSQL(string.Format("SELECT * FROM {0} WHERE GrpID=@GrpID", this.TableName), _params);
    }

    /// <summary>
    /// 取得教師年齡限制
    /// </summary>
    /// <returns></returns>
    public static int GetParameterForAgeLimit()
    {
        try
        {
            return int.Parse(new clsParameters().ParamValue("age_limit"));
        }
        catch
        {
            throw new Exception("age_limit 設定參數有誤");
        }
    }


    protected virtual TeacherQualityualifiODT GeTeacherQualityualifi()
    {
       var result =   this.GetSysParamValue("age_limit", "Iteacher_hour_limit ", "Oteacher_hour_limit");
       return new TeacherQualityualifiODT
       {
           Age_limit = int.Parse(result.FirstOrDefault(p=>p.Key== "age_limit").Value),
           Iteacher_hour_limit = int.Parse(result.FirstOrDefault(p => p.Key == "Iteacher_hour_limit").Value),
           Oteacher_hour_limit = int.Parse(result.FirstOrDefault(p => p.Key == "Oteacher_hour_limit").Value),
       };
    }

    /// <summary>
    /// 取得內聘時數限制
    /// </summary>
    /// <returns></returns>
    public virtual int GetParameterForInTeacherHourLimit()
    {
        try
        {
            return int.Parse(this.ParamValue("Iteacher_hour_limit"));
        }
        catch
        {
            throw new Exception("Iteacher_hour_limit 設定參數有誤");
        }
    }

    /// <summary>
    /// 取得外聘時數限制
    /// </summary>
    /// <returns></returns>
    public virtual int GetParameterForOutTeacherHourLimit()
    {
        try
        {
            return int.Parse(this.ParamValue("Oteacher_hour_limit"));
        }
        catch
        {
            throw new Exception("Oteacher_hour_limit 設定參數有誤");
        }
    }
}