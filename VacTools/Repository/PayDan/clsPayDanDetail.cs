using aiet.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Summary description for clsArea
/// </summary>
public class clsPayDanDetail : BaseTable, IclsPayDanDetail
{

    public clsPayDanDetail()
    {
        this.TableName = "PayDanDetail"; //資料表
        NameField = ""; // 要顯示出來的欄位,例如 Dropdowlist(如不需要可不填)
        pk = "aID"; //真的主鍵欄位
        dfs = new DataFields(pk, NameField);
    }

    public override bool isUsed(string id)
    {
        return false;
    }

    public DataTable GetPayDanDetail(string PayDanID)
    {
        _params.Clear();
        _params.Add(new ListItem("PayDanID", PayDanID));

        DataView dv = selectSQL(@"
select a.aID, a.LessonRID, b.LessonRName, a.TeachHour, a.PracticHour, a.BeforeHour, a.AfterHour,
a.LecturerNo, a.Memo, c.LecturerName, c.Title,b.LessonPro ,a.CrtUser,u.UserName
from PayDanDetail a inner join LessonR b on a.LessonRID=b.LessonRID
left join EducationLecturer c on a.LecturerNo=c.LecturerNo
left join User u on a.CrtUser =  u.UserNo
where a.PayDanID=@PayDanID", _params);
        return dv.Table;
    }
}