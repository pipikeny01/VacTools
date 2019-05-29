using aiet.Base;
using System.Collections.Generic;
using System.Data;
using System.Web.UI.WebControls;
using System.Linq;
using System;
using Aiet_DB;
using aiet.Tools;
using VacWebSiteTools;

/// <summary>
/// Summary description for clsArea
/// </summary>
public class clsLessonSetting : BaseTable
{
    public clsLessonSetting()
    {
        this.TableName = "LessonSetting"; //資料表
        NameField = ""; // 要顯示出來的欄位,例如 Dropdowlist(如不需要可不填)
        pk = "aID"; //真的主鍵欄位
        dfs = new DataFields(pk, NameField);
    }

    public override bool isUsed(string id)
    {
        return false;
    }

    public override string getSNo
    {
        get
        {
            return new clsSerialFmt().getSNo("", 4);
        }
    }

    public virtual DataView GetSettingByLesson(string lessonid)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        this.selectText = string.Format(@"
SELECT a.*,b.Date0,b.Room,b.Teacher,b.LessonRID,lr.LessonRName FROM LessonSetting a inner join LessonRnd b on a.RndID=b.RndID and a.LanNo=b.LanNo
join LessonR lr on lr.LessonRID = b.LessonRID
WHERE a.LessonID = @lessonid");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndByLesson(string lessonid)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        this.selectText = string.Format(@"
SELECT * from vw_LessonRndCheck
WHERE  LessonID = @lessonid");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndByLessonGroup(string lessonid, string group)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        _params.Add(new ListItem("LessonGroup", group));
        this.selectText = string.Format(@"
SELECT * from vw_LessonRndCheck
WHERE  LessonID = @lessonid and (LessonGroup=@LessonGroup or LessonGroup is null)");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndHelperByLesson(string lessonid)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        this.selectText = string.Format(@"
SELECT * from vw_LessonRndHelperCheck
WHERE  LessonID = @lessonid");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndHelperByLessonGroup(string lessonid, string group)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        _params.Add(new ListItem("LessonGroup", group));
        this.selectText = string.Format(@"
SELECT * from vw_LessonRndHelperCheck
WHERE  LessonID = @lessonid and LessonGroup=@LessonGroup");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndExaminationByLesson(string lessonid)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        this.selectText = string.Format(@"
SELECT * from vw_LessonRndExamination
WHERE  LessonID = @lessonid");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndScoringByLesson(string lessonid)
    {
        _params.Clear();
        _params.Add(new ListItem("lessonid", lessonid));
        this.selectText = string.Format(@"
SELECT * from vw_LessonRndScoring
WHERE  LessonID = @lessonid");
        return this.selectSQL(_params);
    }

    public virtual DataView GetViewLessonRndByLessonWeek(string ww,ListItemCollection param)
    {
        var sql = @"
SELECT * from vw_LessonRndCheck
WHERE  1=1  ";
        sql += ww;
        this.selectText = sql;
        return this.selectSQL(param);
    }

    public void UpdateSettings(string rndID, string lessonID, List<string> values)
    {
        if (values == null || values.Count == 0)
            return;

        var trans = new SqlTransHelper();
        trans.CreateCommand("delete from LessonSetting where RndID=@RndID",
            new SqlTransParameter("RndID", rndID));

        var sql = "Insert into LessonSetting (LSettingID,RndID,LanNo,LessonID,CrtUser)values(@LSettingID,@RndID,'zh-tw',@LessonID,@CrtUser)";

        foreach (var v in values)
        {
            trans.CreateCommand(sql,
            new SqlTransParameter("LSettingID", v),
            new SqlTransParameter("RndID", rndID),
            new SqlTransParameter("LessonID", lessonID),
            new SqlTransParameter("CrtUser", MyApp.SysUser.Accnt));
        }

        this.ExecuteTranscation(trans);

        this.updSQL("update Lesson set TotalRndHour=(select count(aid)as count from LessonSetting where LessonID=@LessonID ) where LessonID=@LessonID"
            , new ListItemCollection { new ListItem("LessonID", lessonID) });
    }

    public bool CheckLessonSettingIsExist(string lessonID,string rndID,string lsettingID)
    {
        var sql = "select aid from [LessonSetting] where lessonID=@lessonID and LSettingID=@LSettingID and RndID=@RndID and LanNo=@LanNo";
        this._params.Clear();
        this._params.Add(new ListItem("lessonID", lessonID));
        this._params.Add(new ListItem("LSettingID", lsettingID));
        this._params.Add(new ListItem("RndID", rndID));
        this._params.Add(new ListItem("LanNo", tool.currentLanguage));
        return this.selectSQL(sql, _params).Count>0;

    }



    /// <summary>
    /// 衝堂檢查
    /// </summary>
    /// <param name="param"></param>
    /// <param name="values"></param>
    /// <param name="date0"></param>
    /// <returns></returns>
    public string CheckConflict(CheckSchelExistModel param, List<string> values, string date0)
    {
        var rsql = @"
select * from vw_LessonRndCheck 
where Convert(char(10),Date0,111) = @date0 
and LSettingID in ( Select Data From dbo.fn_slip_str(@LSettingID,','))";

        //找出該時段的資料
        var dv = this.selectSQL(rsql, new ListItemCollection 
        { 
            new ListItem("date0", date0) ,
            new ListItem("LSettingID", string.Join(",",values ))
        });

        //有資料表示該時段已有安排 , 再來查詢教室跟老師使否有安排於此時段
        if(dv.Count==0)
        {
            return "";
        }
        else
        {
           var r = dv.ToTable().AsEnumerable().Where(p => p.Field<string>("Room") == param.Room && p.Field<Guid>("RndID") != new Guid( param.RndID)).ToList();
            if (r.Count>0)
                return string.Format("教室衝堂!\\n{0}於選擇的時段已有安排課程({1})", r.First().Field<string>("AddressName"), r.First().Field<string>("LessonRName"));
            else
            {
                var t = dv.ToTable().AsEnumerable().Where(p => p["Teacher"].ToString() == param.Teacher && p.Field<Guid>("RndID") != new Guid(param.RndID)).ToList();
                if (t.Count > 0)
                    return string.Format("老師衝堂!\\n{0}於選擇的時段已有安排課程({1})", t.First().Field<string>("LecturerName"), t.First().Field<string>("LessonRName"));

            }
        }

        return "";
    }
    public DataView SelectRndHelperCheckSettingDate0(string date0)
    {

        var rsql = @"
select * from vw_LessonRndHelperCheck 
where Convert(char(10),Date0,111) = @date0";

        //找出該班期間的全部資料
        var dv = this.selectSQL(rsql, new ListItemCollection
                        {
                            new ListItem("date0", tools.Cast<DateTime> (date0).ToString("yyyy/MM/dd"))
                        });
        return dv;
    }


    /// <summary>
    /// 衝堂檢查
    /// </summary>
    /// <param name="param"></param>
    /// <param name="values"></param>
    /// <param name="date0"></param>
    /// <returns></returns>
    public string CheckConflict2(CheckSchelExistModel param, List<string> values, string date0, string date1)
    {
        var rsql = @"
select * from vw_LessonRnd 
where Convert(char(10),Date0,111) between @date0 and @date1";

        //找出該班期間的全部資料
        var dv = this.selectSQL(rsql, new ListItemCollection 
        { 
            new ListItem("date0", date0) ,
            new ListItem("date1", date1) 
        });

        //有資料表示該時段已有安排 , 再來查詢教室跟老師使否有安排於此時段
        if (dv.Count == 0)
        {
            return "";
        }
        else
        {
            var r = dv.ToTable().AsEnumerable().Where(p => p.Field<string>("Room") == param.Room && p.Field<Guid>("RndID") != new Guid(param.RndID)).ToList();
            if (r.Count > 0)
                return string.Format("教室衝堂!\\n{0}於選擇的時段已有安排課程({1})", r.First().Field<string>("AddressName"), r.First().Field<string>("LessonRName"));
            else
            {
                var t = dv.ToTable().AsEnumerable().Where(p => p["Teacher"].ToString() == param.Teacher && p.Field<Guid>("RndID") != new Guid(param.RndID)).ToList();
                if (t.Count > 0)
                    return string.Format("老師衝堂!\\n{0}於選擇的時段已有安排課程({1})", t.First().Field<string>("LecturerName"), t.First().Field<string>("LessonRName"));

            }
        }

        return "";
    }

    public DataView SelectRndSetting(string date0,string date1)
    {

        var rsql = @"
select * from vw_LessonRnd 
where DATE_FORMAT(Date0, '%Y/%m/%d') between @date0 and  @date1";

        //找出該班期間的全部資料
        var dv = this.selectSQL(rsql, new ListItemCollection 
                        { 
                            new ListItem("date0", tools.Cast<DateTime> (date0).ToString("yyyy/MM/dd")) ,
                            new ListItem("date1", tools.Cast<DateTime> (date1).ToString("yyyy/MM/dd")) 
                        });
        return dv;
    }

    public DataView SelectRndCheckSetting(string date0,string date1)
    {

        var rsql = @"
select * from vw_LessonRndCheck 
where DATE_FORMAT(Date0,'%Y/%m/%d') between @date0 and  @date1";

        //找出該班期間的全部資料
        var dv = this.selectSQL(rsql, new ListItemCollection 
                        { 
                            new ListItem("date0", tools.Cast<DateTime> (date0).ToString("yyyy/MM/dd")) ,
                            new ListItem("date1", tools.Cast<DateTime> (date1).ToString("yyyy/MM/dd")) 
                        });
        return dv;
    }



    public List<DataRow> CheckRoom(DataTable dv, string room, string date, string settingID, string lessonid = "")
    {

        var r = dv.AsEnumerable().Where(p => p.Field<string>("Room") == room
            && p.Field<DateTime>("Date0").ToString("yyyy/MM/dd") == date
            && p.Field<int>("LSettingID").ToString() == settingID).ToList();
        if (!string.IsNullOrEmpty(lessonid))
        {
            r = dv.AsEnumerable().Where(p => p.Field<string>("Room") == room
            && p.Field<DateTime>("Date0").ToString("yyyy/MM/dd") == date
            && p.Field<int>("LSettingID").ToString() == settingID && p["LessonID"].ToString() != lessonid).ToList();
        }
        return r;
    }

    public List<DataRow> CheckTeacher(DataView dv, string teacher, string date, string settingID, string LessonID = "")
    {
        var r = dv.ToTable().AsEnumerable().Where(p => p["Teacher"].ToString() == teacher
            && p.Field<DateTime>("Date0").ToString("yyyy/MM/dd") == date
            && p.Field<int>("LSettingID").ToString() == settingID).ToList();
        if (!string.IsNullOrEmpty(LessonID))
        {
            r = dv.ToTable().AsEnumerable().Where(p => p["Teacher"].ToString() == teacher
            && p.Field<DateTime>("Date0").ToString("yyyy/MM/dd") == date
            && p.Field<int>("LSettingID").ToString() == settingID && p.Field<string>("LessonID").ToString() != LessonID).ToList();
        }
        return r;
    }

    public List<DataRow> CheckTeacherExamination(DataView dv, string member, string date, string settingID)
    {
        var r = dv.ToTable().AsEnumerable().Where(p => p["Account"].ToString() == member
            && p.Field<DateTime>("Date0").ToString("yyyy/MM/dd") == date
            && p.Field<int>("LSettingID").ToString() == settingID).ToList();
        return r;
    }

    public List<DataRow> CheckTeacherExaminationLecturerId(DataView dv, string teacher, string date, string settingID)
    {
        var r = dv.ToTable().AsEnumerable().Where(p => p["LecturerId"].ToString() == teacher
            && p.Field<DateTime>("Date0").ToString("yyyy/MM/dd") == date
            && p.Field<int>("LSettingID").ToString() == settingID).ToList();
        return r;
    }


    public string getLecturerName(DataTable dt, string aID)
    {
        List<DataRow> cc = dt.AsEnumerable().Where(p => p["aID"].ToString() == aID).ToList();
        List<string> result = new List<string>();
        foreach (DataRow row in cc)
        {
            if (string.IsNullOrEmpty(row["LecturerName"].ToString()))
                continue;
            result.Add(row["LecturerName"].ToString());
        }


        return string.Join("、", result);
    }


    public string getLectureHelpName(DataTable dt, string aID)
    {
        List<DataRow> cc = dt.AsEnumerable().Where(p => p["aID"].ToString() == aID).ToList();
        List<string> result = new List<string>();
        foreach (DataRow row in cc)
        {
            if (string.IsNullOrEmpty(row["LecturerName"].ToString()))
                continue;
            result.Add(row["LecturerName"].ToString());
        }


        return string.Join("、", result);
    }

    public DataView SelectLessonSetting(string copyLessonID)
    {
       return this.selectSQL("select * from LessonSetting where LessonID=@LessonID"
                 , new ListItemCollection { new ListItem("LessonID", copyLessonID) });
    }
}

public class ConflictModel
{
    public string LSettingID { set; get; }
    public string Date { set; get; }
    public string MSG { set; get; }
}

public class CheckSchelExistModel
{
    public string RndID { set; get; }
    public string Teacher { set; get; }
    public string Room { set; get; }
}