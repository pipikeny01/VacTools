using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using aiet.Base;
using VacWebSiteTools;
using Aiet_DB;
using Newtonsoft.Json;
using VacWebSiteTools.DTO;

/// <summary>
/// Summary description for clsArea
/// </summary>
public class clsPayDan : BaseTable
{
    private DataView listName;
    private ListItemCollection _params = new ListItemCollection();
    private DataView dv = null;

    /// <summary>
    /// 提供屬性注入實作IPayDanHelper的物件
    /// </summary>
    public IPayDanHelper PayDanHelper { set; get; }

    public clsPayDan()
    {
        this.TableName = "PayDan"; //資料表
        NameField = "PayDanName"; // 要顯示出來的欄位,例如 Dropdowlist(如不需要可不填)
        pk = "PayDanID"; //真的主鍵欄位
        dfs = new DataFields(pk, NameField);
        ClsPayDanLecturer = new clsPayDanLecturer();
    }

    public override string getSNo
    {
        get
        {
            return new clsSerialFmt().getSNo("PayDan", 10);
        }
    }

    public IclsPayDanLecturer ClsPayDanLecturer { get; set; }

    public bool isExisted(string PayDanID)
    {
        selectText = @"select aID from PayDan where PayDanID=@PayDanID";
        _params.Clear();
        _params.Add(new ListItem("PayDanID", PayDanID));
        return selectSQL(_params).Count == 0 ? false : true;
    }

    public bool isExisted(string aID, string PayDanID)
    {
        selectText = @"select aID from PayDan where PayDanID=@PayDanID and aID <> @ID";
        _params.Clear();
        _params.Add(new ListItem("ID", aID));
        _params.Add(new ListItem("PayDanID", PayDanID));
        return selectSQL(_params).Count == 0 ? false : true;
    }

    public PayDanHourSetDataSource CreatedPayDanHourSetDataSource(string payDanID)
    {
        var payDanHelper = PayDanHelper; //屬性注入的PayDanHeper物件
        var payDanDataSource = new PayDanHourSetDataSource();
        payDanDataSource.CustomDataTableDataSource = new DataTable();
        var panDanDetailDataTable = SelectPayDanDetailDataTable(payDanID);
        if (panDanDetailDataTable.Rows.Count > 0)
        {
            var startDate = Convert.ToDateTime(panDanDetailDataTable.Rows[0]["StartDate"]);
            var endDate = Convert.ToDateTime(panDanDetailDataTable.Rows[0]["EndDate"]);

            var weeks = payDanHelper.GetPayDanWeeks(startDate, endDate);
            payDanDataSource.CustomDataTableDataSource = InitalDataTableColumnFromPayDanWeeks(weeks);
            payDanDataSource.PayDanWeeks = weeks;
            DataSourceAddFromPayDanDetail(payDanID, payDanDataSource.CustomDataTableDataSource, panDanDetailDataTable);
        }

        BindWithPayDanHourSet(payDanDataSource.CustomDataTableDataSource, payDanID);

        return payDanDataSource;
    }

    public DataTable SelectPayDanDetailDataTable(string paydanID)
    {
        var sql = @"
select pd.aID, case ln.lessonPro when 2 then '中心使用時間' when 1 then '專業術科' end as 區分
,ln.LessonRName as 課目 ,'' as 教師, pd.TeachHour as 時數,pd.Memo
,ln.LessonPro,pd.PayDanID,pay.StartDate,pay.EndDate,pay.PayDanName,pay.Verify,pay.StartDate,pay.EndDate
from paydandetail pd
join lessonr ln on pd.lessonrid = ln.LessonRID
join paydan pay on pay.PayDanID=pd.PayDanID
where pd.paydanid  = @paydanid
order by ln.lessonPro desc ,ln.LessonRName";
        _params.Clear();
        _params.Add(new ListItem("paydanid", paydanID));

        return selectSQL(sql, _params).ToTable();
    }

    /// <summary>
    /// 配當表時數設定自定義的DataTable 和 PayDanHourSet的資料做設定對應後 , 就會有存檔的資料了
    /// </summary>
    /// <param name="payDanDataSource"></param>
    /// <param name="payDanID"></param>
    private void BindWithPayDanHourSet(DataTable payDanDataSource, string payDanID)
    {
        var payDanHourSet = this.SelectPayDanHourSet(payDanID);
        foreach (DataRow row in payDanHourSet.Rows)
        {
            var dataSourceRow = payDanDataSource.AsEnumerable().FirstOrDefault(p =>
              p["PayDanDetailaID"].ToString() == row["paydan_detail_id"].ToString());

            if (dataSourceRow != null)
                dataSourceRow[row["Weeks"].ToString()] = row["Hour"];
        }
    }

    private DataTable InitalDataTableColumnFromPayDanWeeks(List<PayDanWeeks> weeks)
    {
        var dt = new DataTable();

        dt.Columns.Add("區分");
        dt.Columns.Add("課目");
        dt.Columns.Add("教師");
        dt.Columns.Add("時數");
        dt.Columns.Add("PayDanDetailaID");
        dt.Columns.Add("StarDate");
        dt.Columns.Add("EndDate");

        foreach (var title in weeks)
        {
            dt.Columns.Add(new DataColumn
            {
                ColumnName = title.Week,
                Caption = string.Format("<div>{0}</div><div>|</div><div>{1}</div>", title.StarDate.ToString("dd"), title.EndDate.ToString("dd"))
            });
        }

        return dt;
    }

    /// <summary>
    /// PayDanDataSource 加入panDanDetailDataTable的資料
    /// </summary>
    /// <param name="payDanID"></param>
    /// <param name="dataSource"></param>
    /// <param name="panDanDetailDataTable"></param>
    private void DataSourceAddFromPayDanDetail(string payDanID, DataTable dataSource, DataTable panDanDetailDataTable)
    {
        var panDanLecturerDataTable = ClsPayDanLecturer.SelectPayDanLecturerDataTable(payDanID);
        foreach (DataRow dr in panDanDetailDataTable.Rows)
        {
            dr["教師"] = BindLecturerCellToString(panDanLecturerDataTable, dr["aID"].ToString());
            dataSource.Rows.Add(dr["區分"],string.Concat(dr["課目"], dr["Memo"]), dr["教師"], dr["時數"], dr["aID"], dr["StartDate"], dr["EndDate"]);
        }
    }

    private string BindLecturerCellToString(DataTable panDanLecturerDataTable, string paydandetail_aid)
    {
        var lecturers = panDanLecturerDataTable.AsEnumerable()
            .Where(p => p["paydandetail_aid"].ToString() == paydandetail_aid)
            .Select(p => CombinLecturerYearsOld(p["lecturerName"].ToString(), p["BirthDay"]))
            .ToList();

        //Select(p => p["lecturerName"].ToString())
        return string.Join(",", lecturers);
    }

    private string CombinLecturerYearsOld(string lecturerName, object birthday)
    {
        var ageHelper = RepositoryFactory.GetAge();
        if (birthday != DBNull.Value)
        {
            return string.Concat(lecturerName, "(",
                    ageHelper.CalculateAge(Convert.ToDateTime(birthday), DateTime.Now).ToString(), ")");
        }
        else
        {
            return lecturerName;
        }
    }

    public DataTable SelectPayDanHourSet(string payDanID)
    {
        var sql = "select * from paydan_hour_set where PayDanID=@PayDanID";
        return this.selectSQL(sql, new ListItemCollection { new ListItem("PayDanID", payDanID) }).ToTable();
    }

    public override bool isUsed(string id)
    {
        selectText = @"select  a.aID from PayDan a inner join PayDanDetail b
  on a.PayDanID = b.PayDanID
  where a.aID =@ID";
        _params.Clear();
        _params.Add(new ListItem("ID", id));
        return selectSQL(_params).Count > 0;
    }

    public DataView SelectPayDanLecturer(string payDanID)
    {
        selectText = @"SELECT ple.*,ele.aID as LecturerId,ele.LecturerName FROM paydan_lecturer ple
inner join educationlecturer ele on ple.LecturerNo = ele.LecturerNo
where PayDanID=@payDanID";
        _params.Clear();
        _params.Add(new ListItem("payDanID", payDanID));
        return selectSQL(_params);
    }

    public DataView SelectPayDanDetail(string lessonid)
    {
        var dv = this.selectSQL(@"
Select * from (
SELECT      pdd.aID as PayDanDetailaID,  pd.PayDanID,   pdd.LessonRID,  lr.LessonRName,lr.LessonPro,el.LecturerName,
pdd.LecturerNo,el.aID as LecturerId ,pdd.Memo
FROM              PayDan AS pd INNER JOIN
                            PayDanDetail AS pdd ON pd.PayDanID = pdd.PayDanID INNER JOIN
                            Lesson AS l ON pd.PayDanID = l.PayDanID INNER JOIN
                            LessonR AS lr ON pdd.LessonRID = lr.LessonRID left JOIN
                            EducationLecturer AS el ON pdd.LecturerNo = el.LecturerNo
Where LessonID=@LessonID

) t  order by PayDanID desc, LessonPro desc

", new ListItemCollection
 {
     new ListItem("LessonID",lessonid)
 });

        return dv;
    }

    public void IniEditLessonRndDropdow(DropDownList ddl, string _lessonID)
    {
        if (ddl.Items.Count == 0)
        {
            var dv = this.SelectPayDanDetail(_lessonID);
            if (dv.Count > 0)
            {
                var dvAllTeachers = SelectPayDanLecturer(dv[0]["PayDanID"].ToString());
                ddl.Items.Add(new ListItem("請輸入科目名稱", ""));
                for (int i = 0; i < dv.Count; i++)
                {
                    var item = new ListItem( string.Concat(dv[i]["LessonRName"],dv[i]["Memo"]), dv[i]["LessonRID"].ToString());
                    //加入老師編號
                    var dbTeachers = SelectTeachersFromAll(dvAllTeachers, dv[i]["PayDanDetailaID"].ToString());
                    var teachers = CreateTeachers(dbTeachers);
                    var teachersJson = JsonConvert.SerializeObject(teachers);
                    item.Attributes.Add("teacher", teachersJson);
                    //item.Attributes.Add("teacher", dv[i]["LecturerId"].ToString() == "0" ? "" : dv[i]["LecturerId"].ToString());
                    //item.Attributes.Add("teacherName", dv[i]["LecturerName"].ToString());
                    ddl.Items.Add(item);
                }
            }
        }
    }

    private List<DataRow> SelectTeachersFromAll(DataView dvTeachers, string payDanDetailaID)
    {
        return dvTeachers.ToTable().AsEnumerable().Where(p => p["paydandetail_aid"].ToString() == payDanDetailaID).ToList();
    }

    private List<Teacher> CreateTeachers(List<DataRow> dbTeachers)
    {
        var teachers = new List<Teacher>();

        for (int i = 0; i < dbTeachers.Count(); i++)
            teachers.Add(new Teacher { TeacherID = dbTeachers[i]["LecturerId"].ToString(), TeacherName = dbTeachers[i]["LecturerName"].ToString() });

        return teachers;
    }


}

public class PayDanHourSetDataSource
{
    public DataTable CustomDataTableDataSource { set; get; }

    public List<PayDanWeeks> PayDanWeeks { set; get; }
}