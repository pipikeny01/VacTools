using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using aiet.Base;

/// <summary>
/// Summary description for clsArea
/// </summary>
public class clsEducationLecturer : BaseTable, IclsEducationLecturer
{
    private clsParameters _parameters;

    public static List<ListItem> TeacherKind
    {
        get
        {
            return new List<ListItem>
            {
                new ListItem { Text = "", Value = "" },
                 new ListItem { Text = "無", Value = "0" },
                 new ListItem { Text = "內聘專任訓練師", Value = "A" },
                 new ListItem { Text = "外聘導師", Value = "B" },
                 new ListItem { Text = "外聘講師", Value = "C" },
            };
        }
    }

    public static List<ListItem> TypeData
    {
        get
        {
            return new List<ListItem>
            {
                new ListItem { Text = "", Value = "" },
                new ListItem { Text = "講師", Value = "1" },
                new ListItem { Text = "助教", Value = "2" },
                new ListItem { Text = "監考人員", Value = "3" },
                new ListItem { Text = "閱卷人員", Value = "4" },
            };
        }
    }

    public static List<ListItem> RetireIdentity
    {
        get
        {
            return new List<ListItem>
            {
                new ListItem { Text = "", Value = "" },
                new ListItem { Text = "軍退", Value = "1" },
                new ListItem { Text = "公退", Value = "2" },
            };
        }
    }


    public clsEducationLecturer()
    {
        this.TableName = "EducationLecturer"; //資料表
        NameField = ""; // 要顯示出來的欄位,例如 Dropdowlist(如不需要可不填)
        pk = "aID"; //真的主鍵欄位
        dfs = new DataFields(pk, NameField);

        _parameters = new clsParameters();

    }

    public override bool isUsed(string id)
    {
        return false;
    }

    public override string getSNo
    {
        get
        {
            return new clsSerialFmt().getSNo("LecturerNo", 10);
        }
    }

    public DataView CheckNoisExisted(string no)
    {
        _params.Clear();
        _params.Add(new ListItem("LecturerNo", no));
        string sql = string.Format("SELECT * FROM {0} WHERE (Account=@LecturerNo or LecturerNo=@LecturerNo) and Disable=0", TableName);
        return this.selectSQL(sql, _params);
    }

    public string GetLecturerID(string accnt)
    {
        _params.Clear();
        _params.Add(new ListItem("Account", accnt));
        string sql = string.Format("SELECT LecturerId FROM {0} WHERE Account=@Account", TableName);
        DataView dv = this.selectSQL(sql, _params);

        if (dv.Count > 0)
            return dv[0]["LecturerId"].ToString();
        else
            return "";
    }

    /// <summary>
    /// 用條件姓名或是編號查詢講師
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public DataView GetLecturerFromNameOrNo(string name)
    {
        _params.Clear();
        _params.Add(new ListItem("LecturerName", name.Trim()));
        string sql = string.Format("SELECT * FROM vw_teacher WHERE (LecturerName=@LecturerName or LecturerNo=@LecturerName) and Disable=0" );
        return this.selectSQL(sql, _params);
    }

    /// <summary>
    /// 編號查詢講師
    /// </summary>
    /// <param lecturerNo="lecturerNo"></param>
    /// <returns></returns>
    public DataView GetLecturerFromNo(string lecturerNo)
    {
        _params.Clear();
        _params.Add(new ListItem("LecturerName", lecturerNo.Trim()));
        string sql = string.Format("SELECT * FROM vw_teacher WHERE ( LecturerNo=@LecturerName) and Disable=0");
        return this.selectSQL(sql, _params);
    }

    /// <summary>
    /// 編號查詢講師
    /// </summary>
    /// <param aid="aid"></param>
    /// <returns></returns>
    public DataView GetLecturerFromaID(string aid)
    {
        _params.Clear();
        _params.Add(new ListItem("LecturerName", aid));
        string sql = string.Format("SELECT * FROM vw_teacher WHERE ( aID=@aid) and Disable=0");
        return this.selectSQL(sql, _params);
    }


    /// <summary>
    /// 查詢教師日期區間的時數
    /// </summary>
    /// <param name="aid"></param>
    /// <param name="d1"></param>
    /// <param name="d2"></param>
    /// <returns></returns>
    public DataView SelectTeacherHourCount(string aid , string d1 , string d2)
    {
        _params.Clear();
        _params.Add(new ListItem("LecturerName", aid));
        _params.Add(new ListItem("d1", d1));
        _params.Add(new ListItem("d2", d2));
        string sql = string.Format(@"
select teacher,LecturerName,LecturerNo,yearsold,count(teacher)HourCount from vw_lessonrndcheck
where  DATE_FORMAT(Date0,'%Y/%m/%d') between @d1  and  @d2
group by  teacher,LecturerName,LecturerNo,yearsold
");
        return this.selectSQL(sql, _params);

    }


    public bool CheckLecturerIDNO_Exist(string idno)
    {
        return GetLecturerFromIDNO(idno).Count > 0;
    }

    public DataView GetLecturerFromIDNO(string idno)
    {
        _params.Clear();
        _params.Add(new ListItem("IDNO", idno.Trim()));
        string sql = string.Format("SELECT * FROM EducationLecturer WHERE {0} = @IDNO and Disable=0 ", clsRSA.DecryptForMySql("IDNO"));
        return this.selectSQL(sql, _params);
    }

    /// <summary>
    /// 驗證教師分類不是無 , 身分證跟Email必填
    /// </summary>
    /// <param name="teacherKind"></param>
    /// <param name="idno"></param>
    /// <param name="birthday"></param>
    /// <returns></returns>
    public bool ValidateIDNOPass(string teacherKind, string idno, string birthday)
    {
        if (teacherKind != "0")
        {
            return !string.IsNullOrEmpty(idno) && !string.IsNullOrEmpty(birthday);
        }

        return true;
    }

    /// <summary>
    /// 檢查是不是超過限制年齡
    /// </summary>
    /// <param name="teacherNo"></param>
    /// <returns></returns>
    public bool CheckYearsOldLimitIsOver(string teacherNo)
    {
        var yearLimit = clsParameters.GetParameterForAgeLimit();
        var dvTeacher = this.GetLecturerFromNameOrNo(teacherNo);
        if (dvTeacher.Count > 0)
        {
            if (dvTeacher[0]["yearsold"] == DBNull.Value)
                return false;

            return Convert.ToInt16(dvTeacher[0]["yearsold"]) > yearLimit;
        }

        return true;
    }


    //    public DataView  AddTeacherByMember(string no)
    //    {
    //        var dvMember = this.selectSQL(string.Format(@"
    //SELECT          m.MemberName, m.UMember, m.Email, j.JTName, {0} as UID
    //FROM              Member AS m INNER JOIN
    //                 JobTitle AS j ON m.JTID = j.JTID
    //where UMember = @UMember", clsRSA.Decrypt("m.UID")), new ListItemCollection { new ListItem("UMember", no) });

    //        if (dvMember.Count != 0)
    //        {
    //            this.instSQL(string.Format(@"insert into EducationLecturer (IDNO,LecturerNo,LecturerName,Account,Title,Email,In_Out,Disable,CrtTime,CrtUser,ModTime,ModUser)
    //            values({0},@LecturerNo,@LecturerName,@Account,@Title,@Email,0,0,now(),@CrtUser,now(),@ModUser)", clsRSA.Encrypt("@IDNO"))
    //                , new ListItemCollection
    //                {
    //                    new ListItem("IDNO",dvMember[0]["UID"].ToString()),
    //                    new ListItem("LecturerNo",new clsEducationLecturer().getSNo),
    //                    new ListItem("LecturerName",dvMember[0]["MemberName"].ToString()),
    //                    new ListItem("Account",no),
    //                    new ListItem("Title",dvMember[0]["JTName"].ToString()),
    //                    new ListItem("Email",dvMember[0]["Email"].ToString()),
    //                    new ListItem("CrtUser",App.SysUser.Accnt),
    //                    new ListItem("ModUser",App.SysUser.Accnt),

    //                });

    //            dvMember = CheckNoisExisted(no);
    //        }

    //        return dvMember;
    //    }
}

public static class TeacherKindDefine
{
    public static string 內聘專任訓練師
    {
        get { return "A"; }
    }
    public static string 外聘導師
    {
        get { return "B"; }
    }

    public static string 外聘講師
    {
        get { return "C"; }
    }

}
