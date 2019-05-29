using System;
using System.ComponentModel;
using System.Data;
using System.Web.UI.WebControls;
using aiet.Base;

public class clsPayDanLecturer : BaseTable, IclsPayDanLecturer
{
    public enum AddStatus
    {
        [Description("教師已經存在,請重新選擇")]
        Exist,
        [Description("")]
        Success,
        [Description("教師年齡超過限制,請重新選擇")]
        YearsOldLimit
    }

    private clsEducationLecturer _educationLecturer;

    public clsPayDanLecturer()
    {
        TableName = "paydan_lecturer";
        NameField = "";
        pk = "aID";

        _educationLecturer = new clsEducationLecturer();
    }

    public DataTable SelectPayDanLecturerDataTable(string paydanID)
    {
        var sql = @"
select plec.aID, plec.paydandetail_aid,lec.lecturerNo,lec.lecturerName,lec.BirthDay
from paydan_lecturer plec
join educationlecturer lec
on plec.lecturerNo = lec.lecturerNo
where PayDanID=@PayDanID";
        _params.Clear();
        _params.Add(new ListItem("PayDanID", paydanID));
        return selectSQL(sql, _params).ToTable();
    }

    public virtual AddStatus Add(string payDanID, string payDetailID, string teacherNo, string user)
    {
        if (this.CheckLecturerExistPaydanDetail(payDetailID, teacherNo))
        {
            return AddStatus.Exist;
        }

        if (_educationLecturer.CheckYearsOldLimitIsOver(teacherNo))
        {
            return AddStatus.YearsOldLimit;
        }

        var sql = @"
insert into paydan_lecturer (PayDanID,paydandetail_aid,lecturerNo,CrtTime,CrtUser) values
(@PayDanID,@paydandetail_aid,@lecturerNo,now(),@CrtUser)
";
        _params.Clear();
        _params.Add(new ListItem("PayDanID", payDanID));
        _params.Add(new ListItem("paydandetail_aid", payDetailID));
        _params.Add(new ListItem("lecturerNo", teacherNo));
        _params.Add(new ListItem("CrtUser", user));
        instSQL(sql, _params);

        return AddStatus.Success;
    }

    /// <summary>
    /// 檢查教師是不是已經存在Detail中
    /// </summary>
    /// <param name="payDetailId"></param>
    /// <param name="teacherId"></param>
    /// <returns></returns>
    public virtual bool CheckLecturerExistPaydanDetail(string payDetailId, string teacherId)
    {
        var sql = "select aid from paydan_lecturer where paydandetail_aid=@paydandetail_aid and lecturerNo=@lecturerNo";
        _params.Clear();
        _params.Add(new ListItem("paydandetail_aid", payDetailId));
        _params.Add(new ListItem("lecturerNo", teacherId));
        return this.selectSQL(sql, _params).Count > 0 ? true : false;
    }
}