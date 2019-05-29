using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using aiet.Base;
using aiet.Tools;
using Newtonsoft.Json;
using System.Linq;
using Aiet_DB;
using VacWebSiteTools.Service;
using VacWebSiteTools;

public class clsLesson : BaseTable
{
    public DataTable dtTable;

    /// <summary>
    /// 權限列表，空白不顯示
    /// </summary>
    public string GridView = "";


    /// <summary>
    /// 班對類別代碼A 終身學習 B 服務業 C 業務管理 D 資訊能力提升 E 創業技能提升 Z 新課程
    /// </summary>
    public static List<ListItem> LNID
    {
        get
        {
            return new List<ListItem>
            {
                 new ListItem { Text = "終身學習", Value = "A" },
                 new ListItem { Text = "服務業", Value = "B" },
                 new ListItem { Text = "業務管理", Value = "C" },
                 new ListItem { Text = "資訊能力提升", Value = "D" },
                 new ListItem { Text = "創業技能提升", Value = "E" },
                 new ListItem { Text = "新課程", Value = "Z" },
            };
        }
    }

    public static List<ListItem> LSchID
    {
        get
        {
            return new List<ListItem>
            {
                 new ListItem { Text = "平日班", Value = "S1" },
                 new ListItem { Text = "假日班", Value = "S2" },
                 new ListItem { Text = "夜班", Value = "S3" },
            };
        }
    }

    /// <summary>
    /// 開班狀況進度
    /// </summary>
    public static List<ListItem> Class_states
    {
        get
        {
            return new List<ListItem>
            {
                 new ListItem { Text = "計畫", Value = "A" },
                 new ListItem { Text = "決標", Value = "B" },
                 new ListItem { Text = "報名", Value = "C" },
                 new ListItem { Text = "開班", Value = "D" },
                 new ListItem { Text = "完訓", Value = "E" },
            };
        }
    }


    public clsLesson()
    {
        //
        // TODO: 在此加入建構函式的程式碼
        //
        this.TableName = "Lesson";
        NameField = "LessonName";
        pk = "LessonID";
        dfs = new DataFields("LessonID", "LessonName");

    }

    public override string getSNo
    {
        get
        {
            return new clsSerialFmt().getSNo("Lesson", 6);
        }
    }


    public DataModel GetDataPager(string sql, int pageIndex, int pageSize, string ww = "", ListItemCollection param = null, string othersql = "")
    {
        if (param == null) param = new ListItemCollection();

        return AddPagerScriptOther(string.Format(sql, ww), pageIndex, pageSize, param, true, othersql);
    }

    public DataModel GetDataPager_detail(int pageIndex, int pageSize, string ww = "", ListItemCollection param = null)
    {
        if (param == null) param = new ListItemCollection();

        return AddPagerScript(string.Format(@"
select Row_Number() over(order by t.Date0, t.T1)RowIndex, * from (
select a.LessonName,a.RndID, a.LessonRName, a.Date0, a.LecturerName, a.AddressName, a.Address , b.FileUrl
, Min(a.T1) T1, Max(a.T2) T2, 
--Round(Datediff(MINUTE, Min(a.T1), Max(a.T2))/60.0,1) totalHour 
 count(a.T1) totalHour
from vw_LessonRndCheck a
left join LessonAttachment b on a.LessonID=b.LessonID and a.RndID=b.RndID
where 1=1 {0} group by a.LessonName,a.RndID, a.LessonRName, a.Date0, a.LecturerName, a.AddressName, a.Address, b.FileUrl)t ", ww), pageIndex, pageSize, param, true);
    }

    /// <summary>
    ///  課表
    /// </summary>
    /// <param name="LessonID"></param>
    /// <returns></returns>
    public DataTable RndList(string LessonID, string where = "", string orderBy = "order by CrtTime desc", List<ListItem> param = null)
    {
        selectText = @"
select ROW_NUMBER() OVER(##rownumber) AS ROWID ,* from (
SELECT          a.aID, a.RndID, a.LanNo, a.LessonID, a.Locate, a.Room, a.Date0, a.Date1, a.ClsQty, a.ClsTime, a.Contactor, a.Tel,
                            a.Mobile, a.Email, a.Teacher, a.Food, a.Agendar, a.CrtTime, a.CrtUser, a.ModTime, a.ModUser, a.Guid, a.Obligatory,
                            a.MaterialFile, b.LecturerName, ea.AddressName, a.LessonRID, lr.LessonRName, lrk.LessonRKindName
FROM              LessonRnd AS a INNER JOIN
                            LessonR AS lr ON a.LessonRID = lr.LessonRID INNER JOIN
                            LessonRKind AS lrk ON lr.LessonRKindID = lrk.LessonRKindID LEFT OUTER JOIN
                            EducationLecturer AS b ON a.Teacher = b.LecturerId AND a.LanNo = b.LanNo LEFT OUTER JOIN
                            EducationAddress AS ea ON a.Room = ea.aID
) t
where LessonID=@LessonID and LanNo=@LanNo
##where
";
        selectText = selectText.Replace("##rownumber", orderBy);
        selectText = selectText.Replace("##where", where);
        selectText += orderBy;
        _params.Clear();
        _params.Add(new ListItem("LessonID", LessonID));
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        if (param != null)
            _params.AddRange(param.ToArray());
        return selectSQL(_params).Table;
    }

    /// <summary>
    /// 上課模式
    /// </summary>
    public static List<ListItem> LSType
    {
        get
        {
            List<ListItem> li = new List<ListItem>();
            li.Add(new ListItem("", ""));
            li.Add(new ListItem(MyResources.CourseResource.GetString("LSType0"), "0"));
            //li.Add(new ListItem(Resources.CourseResource.LSType1, "1"));
            li.Add(new ListItem(MyResources.CourseResource.GetString("LSType2"), "1"));
            return li;
        }
    }

    /// <summary>
    ///  附件
    /// </summary>
    /// <param name="LessonID"></param>
    /// <returns></returns>
    public DataTable Attachment(string LessonID)
    {
        selectText = @"select * from LessonAttachment  where LessonID=@LessonID and LanNo=@LanNo ";
        _params.Clear();
        _params.Add(new ListItem("LessonID", LessonID));
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        return selectSQL(_params).Table;
    }

    public string RndName(string LessonID, string RndID)
    {
        selectText = @"select RndName from LessonRnd  where LessonID=@LessonID and LanNo=@LanNo and RndID=@RndID";
        _params.Clear();
        _params.Add(new ListItem("LessonID", LessonID));
        _params.Add(new ListItem("RndID", RndID));
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        DataView dv = selectSQL(_params);
        return dv.Count == 0 ? "" : dv[0][0].ToString();
    }

    /// <summary>
    ///  隨動態欄位顯示
    /// </summary>
    /// <param name="LessonID"></param>
    /// <returns></returns>
    public string ExpressSqlDynamic(string LessonID)
    {
        ListItemCollection pp = new ListItemCollection();
        pp.Add(new ListItem("LessonID", LessonID));
        DataView dv = selectSQL("select * from  LessonFields where LessonID=@LessonID order by Sort", pp);
        StringBuilder expression = new StringBuilder();
        for (int i = 0; i < dv.Count; i++)
        {
            if (dv[i]["FieldName"].ToString() == "UID")
            {
                string UID = clsRSA.Decrypt(string.Format("{0}.{1}", dv[i]["TableName"], dv[i]["FieldName"]));
                expression.AppendFormat(",{0} as {1}", UID, dv[i]["FieldDesc"]);
            }
            else if (dv[i]["FieldName"].ToString() == "Birthday")
            {
                expression.AppendFormat(",Convert(char(10),{0}.{1},111) as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
            else if (dv[i]["FieldName"].ToString() == "JTID")
            {
                expression.AppendFormat(",JobTitle.JTName as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
            else if (dv[i]["FieldName"].ToString() == "CareerID")
            {
                expression.AppendFormat(",Career.CareerName as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
            else if (dv[i]["FieldName"].ToString() == "Sex")
            {
                expression.AppendFormat(",case Member.Sex when 'M' then '男' when 'F' then '女' else '' end as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
            else if (dv[i]["FieldName"].ToString() == "MbrID")
            {
                expression.AppendFormat(",MbrType.MbrName as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
            else if (dv[i]["FieldName"].ToString() == "SubID")
            {
                expression.AppendFormat(", MbrTypeExt.SubName  as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
            else
            {
                expression.AppendFormat(",{0}.{1} as {2}", dv[i]["TableName"], dv[i]["FieldName"], dv[i]["FieldDesc"]);
            }
        }

        return string.Format(@"select LessonRegister.aID  MemberNo,IndustryType.IndTypeName as {0},LessonRnd.RndName as {1} {2}
    , LessonRegister.Attachment as {10},case isnull(LessonRegister.Status,0) when 0 then '{3}' when 1 then '{4}' when 2 then '{5}' when 3 then '{6}' when 4 then '{7}' else '{8}' end {9}
    from LessonRegister
    left join IndustryType on LessonRegister.IndTypeID = IndustryType.IndTypeID
    left join LessonRnd
    on LessonRegister.LessonID = LessonRnd.LessonID and LessonRegister.RndID = LessonRnd.RndID
    left join Member on LessonRegister.MemberNo = Member.MemberNo
    left join MemberExt on LessonRegister.MemberNo = MemberExt.MemberNo
    left join LessonRegExt on LessonRegister.Num = LessonRegExt.Num
    left join Career on Member.CareerID = Career.CareerID
    left join JobTitle on Member.JTID = JobTitle.JTID
    left join MbrType on Member.MbrID = MbrType.MbrID and  Member.LanNo = MbrType.LanNo
    left join MbrTypeExt on Member.MbrID = MbrTypeExt.MbrID and Member.SubID = MbrTypeExt.SubID and  Member.LanNo = MbrType.LanNo
    where 1=1
    ", MyResources.BannerResource.GetString("Industry"), MyResources.CourseResource.GetString("RndName"), expression.ToString()
         , MyResources.BaseResource.GetString("Pending"), MyResources.BaseResource.GetString("Approved"), MyResources.BaseResource.GetString("Backpieces"), MyResources.BaseResource.GetString("Forbid"),
         MyResources.BaseResource.GetString("Cancel"), "-"
         , MyResources.BaseResource.GetString("AuditStatus"), MyResources.BaseResource.GetString("txtAttachment"));
    }

    /// <summary>
    /// -- 基本會員欄位
    /// </summary>
    /// <param name="LessonID"></param>
    /// <returns></returns>
    public string ExpressSql(string LessonID)
    {
        ListItemCollection pp = new ListItemCollection();
        pp.Add(new ListItem("LessonID", LessonID));

        //StringBuilder expression = new StringBuilder();
        //expression.AppendFormat("{0} {1}", clsRSA.Decrypt("Member.UID"), Resources.MemberResource.UID);
        //expression.AppendFormat(",Member.MemberName {0}", Resources.MemberResource.MemberName);
        //expression.AppendFormat(",convert(char(10),Member.Birthday,111) {0},Member.JTName {1} ,Member.MbrName {2} ,Member.SubName {3}", Resources.MemberResource.Birthday, Resources.MemberResource.JTName, Resources.MemberResource.MbrName, Resources.MemberResource.Career);
        //expression.AppendFormat(",case Member.Sex when 'M' then '男' when 'F' then '女' else '' end as {0}", Resources.MemberResource.Sex);
        //expression.AppendFormat(",Member.Tel {0},Member.Mobile {1}", Resources.MemberResource.Tel, Resources.MemberResource.Mobile);

        return string.Format(@"select Distinct b.UMember as 員工編號,  b.OUID ,d.JTID,cr.CrtUser,u.Dept,a.LessonID,a.MemberNo,a.Status,a.CancelTime,b.UMember {0}, b.MemberName {1},case b.Sex when '1' then '男' when '2' then '女' else '' end {2},
    c.OUName {3}, d.JTName {4}, b.Email {5}, case when dbo.TRIM(isnull(b.Tel,''), '-') = '' then '' else dbo.R_TRIM(dbo.L_TRIM(isnull(b.AreaCode,'')+'-'+b.Tel+'*'+isnull(b.Fax,''), '-'), '*') end {6},
    b.Mobile {7} from LessonRegister a
    inner join LessonClassmateReg cr on a.MemberNo=cr.MemberNo  and a.LessonID = cr.LessonID
    inner join [User] u on cr.CrtUser =  u.UserNo
    inner join Member b on a.MemberNo=b.MemberNo
    left join Organize c on b.OUID=c.OUID
    left join JobTitle d on b.JTID=d.JTID
    --where (b.Disable=0 or b.Disable is null)
    where 1 =1
    ", MyResources.MemberResource.GetString("UMember"), MyResources.MemberResource.GetString("MemberName"), MyResources.MemberResource.GetString("Sex")
    , MyResources.BaseResource.GetString("OUName"), MyResources.MemberResource.GetString("JTName"), MyResources.MemberResource.GetString("Email"),
        MyResources.MemberResource.GetString("Tel"), MyResources.MemberResource.GetString("Mobile"));
    }

    /// <summary>
    /// -- 基本會員欄位
    /// </summary>
    /// <param name="LessonID"></param>
    /// <returns></returns>
    //public string ExpressSql_Status(string LessonID)
    //{
    //    ListItemCollection pp = new ListItemCollection();
    //    pp.Add(new ListItem("LessonID", LessonID));

    //    //StringBuilder expression = new StringBuilder();
    //    //expression.AppendFormat("{0} {1}", clsRSA.Decrypt("Member.UID"), Resources.MemberResource.UID);
    //    //expression.AppendFormat(",Member.MemberName {0}", Resources.MemberResource.MemberName);
    //    //expression.AppendFormat(",convert(char(10),Member.Birthday,111) {0},Member.JTName {1} ,Member.MbrName {2} ,Member.SubName {3}", Resources.MemberResource.Birthday, Resources.MemberResource.JTName, Resources.MemberResource.MbrName, Resources.MemberResource.Career);
    //    //expression.AppendFormat(",case Member.Sex when 'M' then '男' when 'F' then '女' else '' end as {0}", Resources.MemberResource.Sex);
    //    //expression.AppendFormat(",Member.Tel {0},Member.Mobile {1}", Resources.MemberResource.Tel, Resources.MemberResource.Mobile);

    //    return string.Format(@"select Distinct b.MemberNo, b.UMember {0}, b.MemberName {1},case b.Sex when '1' then '男' when '2' then '女' else '' end {2},
    //c.OUName {3}, d.JTName {4}, b.Email {5}, case when dbo.TRIM(isnull(b.Tel,''), '-') = '' then '' else dbo.R_TRIM(dbo.L_TRIM(isnull(b.AreaCode,'')+'-'+b.Tel+'*'+isnull(b.Fax,''), '-'), '*') end {6},
    //b.Mobile {7}, case isnull(a.Status,0) when 0  then '{8}' when 1 then '{9}' when 2 then '{10}' when 3 then '{11}' when 4 then '{12}' else '{13}'  end {14} from LessonRegister a
    //inner join Member b on a.MemberNo=b.MemberNo
    //left join Organize c on b.OUID=c.OUID
    //left join JobTitle d on b.JTID=d.JTID
    //where (b.Disable=0 or b.Disable is null)
    //", Resources.MemberResource.UMember, Resources.MemberResource.MemberName, Resources.MemberResource.Sex, Resources.BaseResource.OUName, Resources.MemberResource.JTName, Resources.MemberResource.Email,
    //    Resources.MemberResource.Tel, Resources.MemberResource.Mobile, Resources.BaseResource.Pending, Resources.BaseResource.Approved, Resources.BaseResource.Backpieces, Resources.BaseResource.Forbid, Resources.BaseResource.Cancel, "-"
    //     , Resources.BaseResource.AuditStatus);
    //}

    public void DynamicColumn_Status(GridView gv, SqlDataSource _sds)
    {
        Page page = (Page)HttpContext.Current.CurrentHandler;

        gv.Columns.Clear();

        DataTable dt = ((DataView)_sds.Select(new DataSourceSelectArguments())).Table;

        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (dt.Columns[i].ColumnName == "MemberNo")
            {
                string hn = @"<input type='checkbox' onclick=""$('.cxMember').prop('checked',$(this).prop('checked'))"") />";

                GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

                GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
                l1.dataBinding += delegate (object sender, Dictionary<string, string> row)
                {

                    ((Literal)sender).Text = string.Format("<input type='checkbox' class='cxMember' name='cxMember' value='{0}'/>", row["MemberNo"]);
                };

                TemplateField tm = new TemplateField();
                tm.HeaderTemplate = l0;
                tm.ItemTemplate = l1;
                gv.Columns.Insert(0, tm);
                continue;
            }

            if (dt.Columns[i].Caption.Equals(MyResources.BaseResource.GetString("txtAttachment")))
            {
                TemplateField tf = new TemplateField();
                tf.HeaderText = dt.Columns[i].Caption;
                string CN = dt.Columns[i].ColumnName;
                GridViewTemplate<Literal> li = new GridViewTemplate<Literal>(DataControlRowType.DataRow, CN);
                li.dataBinding += delegate (object sender, Dictionary<string, string> row)
                {
                    ((Literal)sender).Text = string.Format("<a href='{0}?f={1}/{2}/{3}' ><img src='{4}' border='0' /></a>", page.ResolveUrl("~/lib/download.ashx"), "~/upload/lesson/", page.Request.QueryString["ID"], row[CN], page.ResolveUrl("~/assistportal/images/detail.gif"));
                };

                tf.ItemTemplate = li;
                //                    tf.ItemStyle.Width = 80;
                gv.Columns.Add(tf);
            }
            else
            {
                BoundField bf = new BoundField();
                bf.HeaderText = dt.Columns[i].Caption;
                bf.DataField = dt.Columns[i].ColumnName;
                //                    bf.ItemStyle.Width = 80;
                gv.Columns.Add(bf);
            }
        }
    }

    //    /// <summary>
    //    /// 場次資料
    //    /// </summary>
    //    /// <param name="LessonID"></param>
    //    /// <param name="RndID"></param>
    //    /// <returns></returns>
    //    public DataView Round(string LessonID, string RndID)
    //    {
    //        _params.Clear();
    //        _params.Add(new ListItem("LessonID", LessonID));
    //        _params.Add(new ListItem("RndID", RndID));

    //        selectText = string.Format(@"select a.LessonID,b.RndID,c.LKName,a.LessonName,case a.LSType when '0' then '{0}' when '1' then '{1}' end LSType,a.Organizer,b.Teacher
    //,b.Date0,b.Date1,b.ClsTime,b.RndName,a.Cost,a.Subject,a.RDate0,a.RDate1,b.ClsQty,a.isFull,b.Contactor,b.Email
    //,b.Tel,b.Mobile,a.Memo,a.Summary,b.Locate,b.Room,b.Agendar, lrr.Status, b.Date0
    // from Lesson a left join LessonRnd b on a.LessonID = b.LessonID and a.LanNo = b.LanNo
    // inner join LessonKind c on a.LanNo = c.LanNo and a.LKID = c.LKID
    //left join (select Status,LessonID,RndID from LessonRegister where Status <> '4' {2}) lrr on a.LessonID = lrr.LessonID AND b.RndID = lrr.RndID
    //where 1=1 and a.LessonID=@LessonID and b.RndID=@RndID  ", Resources.CourseResource.LSType0, Resources.CourseResource.LSType0, "{0}");
    //        if (App.Member != null)
    //        {
    //            selectText = string.Format(selectText, " AND MemberNo=@MemberNo");
    //            _params.Add(new ListItem("MemberNo", App.Member.MemberNo));
    //        }
    //        else
    //        {
    //            selectText = string.Format(selectText, "");
    //        }

    //        return selectSQL(_params);
    //    }

    public void DynamicColumn(GridView gv, SqlDataSource _sds)
    {
        Page page = (Page)HttpContext.Current.CurrentHandler;

        gv.Columns.Clear();

        DataTable dt = ((DataView)_sds.Select(new DataSourceSelectArguments())).Table;

        for (int i = 0; i < dt.Columns.Count; i++)
        {
            if (dt.Columns[i].ColumnName == "MemberNo")
            {
                string hn = @"<input type='checkbox' onclick=""$('.cxMember').prop('checked',$(this).prop('checked'))"" />";

                GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

                GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
                l1.dataBinding += delegate (object sender, Dictionary<string, string> row)
                {
                    ((Literal)sender).Text = row[MyResources.BaseResource.GetString("AuditStatus")]
                    == MyResources.BaseResource.GetString("Pending") ? 
                    string.Format("<input type='checkbox' class='cxMember' name='cxMember' value='{0}'/>", row["MemberNo"]) : "";
                };

                TemplateField tm = new TemplateField();
                tm.HeaderTemplate = l0;
                tm.ItemTemplate = l1;
                gv.Columns.Insert(0, tm);
                continue;
            }

            if (dt.Columns[i].Caption.Equals(MyResources.BaseResource.GetString("txtAttachment")))
            {
                TemplateField tf = new TemplateField();
                tf.HeaderText = dt.Columns[i].Caption;
                string CN = dt.Columns[i].ColumnName;
                GridViewTemplate<Literal> li = new GridViewTemplate<Literal>(DataControlRowType.DataRow, CN);
                li.dataBinding += delegate (object sender, Dictionary<string, string> row)
                {
                    ((Literal)sender).Text = string.Format("<a href='{0}?f={1}/{2}/{3}' ><img src='{4}' border='0' /></a>", page.ResolveUrl("~/lib/download.ashx"), "~/upload/lesson/", page.Request.QueryString["ID"], row[CN], page.ResolveUrl("~/assistportal/images/detail.gif"));
                };

                tf.ItemTemplate = li;
                //                    tf.ItemStyle.Width = 80;
                gv.Columns.Add(tf);
            }
            else
            {
                BoundField bf = new BoundField();
                bf.HeaderText = dt.Columns[i].Caption;
                bf.DataField = dt.Columns[i].ColumnName;
                //                    bf.ItemStyle.Width = 80;
                gv.Columns.Add(bf);
            }
        }
    }

    //public void DynamicColumn_Student(GridView gv, SqlDataSource _sds)
    //{
    //    Page page = (Page)HttpContext.Current.CurrentHandler;

    //    gv.Columns.Clear();

    //    DataTable dt = ((DataView)_sds.Select(new DataSourceSelectArguments())).Table;

    //    for (int i = 0; i < dt.Columns.Count; i++)
    //    {
    //        if (dt.Columns[i].ColumnName == "aID")
    //        {
    //            string hn = @"<input type='checkbox' onclick=""$('.cxMember').prop('checked',$(this).prop('checked'))"" />";

    //            GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

    //            GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
    //            l1.dataBinding += delegate(object sender, Dictionary<string, string> row)
    //            {
    //                bool has;
    //                if (!bool.TryParse(row["課程通過狀態"], out has))
    //                    has = false;

    //                //var has2 = row["課程通過狀態"] ==null ?false: Convert.ToBoolean(row["課程通過狀態"]);
    //                ((Literal)sender).Text = string.Format("<input type='checkbox' class='cxMember' name='cxMember' value='{0}' {1}/><input type='hidden' name='hdaID' value='{0}'/>", row["aID"], has ? "checked" : "");
    //            };

    //            TemplateField tm = new TemplateField();
    //            tm.HeaderTemplate = l0;
    //            tm.ItemTemplate = l1;
    //            gv.Columns.Insert(0, tm);
    //            continue;
    //        }

    //        if (dt.Columns[i].ColumnName == Resources.LessonResource.EXScore1)
    //        {
    //            string hn = Resources.LessonResource.EXScore1;

    //            GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

    //            GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
    //            l1.dataBinding += delegate(object sender, Dictionary<string, string> row)
    //            {
    //                ((Literal)sender).Text = string.Format("<input type='text' class='txtEXScore1' name='txtEXScore1' value='{0}'/>", row[Resources.LessonResource.EXScore1]);
    //            };

    //            TemplateField tm = new TemplateField();
    //            tm.HeaderTemplate = l0;
    //            tm.ItemTemplate = l1;
    //            gv.Columns.Add(tm);
    //            continue;
    //        }

    //        if (dt.Columns[i].ColumnName == Resources.LessonResource.EXScore2)
    //        {
    //            string hn = Resources.LessonResource.EXScore2;

    //            GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

    //            GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
    //            l1.dataBinding += delegate(object sender, Dictionary<string, string> row)
    //            {
    //                ((Literal)sender).Text = string.Format("<input type='text' class='txtEXScore2' name='txtEXScore2' value='{0}'/>", row[Resources.LessonResource.EXScore2]);
    //            };

    //            TemplateField tm = new TemplateField();
    //            tm.HeaderTemplate = l0;
    //            tm.ItemTemplate = l1;
    //            gv.Columns.Add(tm);
    //            continue;
    //        }

    //        if (dt.Columns[i].ColumnName == Resources.CourseResource.Memo)
    //        {
    //            string hn = Resources.CourseResource.Memo;

    //            GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

    //            GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
    //            l1.dataBinding += delegate(object sender, Dictionary<string, string> row)
    //            {
    //                ((Literal)sender).Text = string.Format("<textarea name='txtmemo' rows='3' style='width: 300px'>{0}</textarea>", row[Resources.CourseResource.Memo]);
    //            };

    //            TemplateField tm = new TemplateField();
    //            tm.HeaderTemplate = l0;
    //            tm.ItemTemplate = l1;
    //            gv.Columns.Add(tm);
    //            continue;
    //        }

    //        if (dt.Columns[i].ColumnName == Resources.CourseResource.isPass)
    //        {
    //            string hn = Resources.CourseResource.isPass;

    //            GridViewTemplate<Literal> l0 = new GridViewTemplate<Literal>(DataControlRowType.Header, hn);

    //            GridViewTemplate<Literal> l1 = new GridViewTemplate<Literal>(DataControlRowType.DataRow, dt.Columns[i].ColumnName, dt.Columns);
    //            l1.dataBinding += delegate(object sender, Dictionary<string, string> row)
    //            {
    //                Literal isPass = ((Literal)sender);
    //                string status = string.IsNullOrWhiteSpace(row[Resources.CourseResource.isPass]) ? "False" : row[Resources.CourseResource.isPass];

    //                if (bool.Parse(status))
    //                    isPass.Text = Resources.BaseResource.Approved;
    //                else
    //                    isPass.Text = Resources.BaseResource.Forbid;
    //            };

    //            TemplateField tm = new TemplateField();
    //            tm.HeaderTemplate = l0;
    //            tm.ItemTemplate = l1;
    //            gv.Columns.Add(tm);
    //            continue;
    //        }

    //        if (dt.Columns[i].Caption.Equals(Resources.BaseResource.txtAttachment))
    //        {
    //            TemplateField tf = new TemplateField();
    //            tf.HeaderText = dt.Columns[i].Caption;
    //            string CN = dt.Columns[i].ColumnName;
    //            GridViewTemplate<Literal> li = new GridViewTemplate<Literal>(DataControlRowType.DataRow, CN);
    //            li.dataBinding += delegate(object sender, Dictionary<string, string> row)
    //            {
    //                ((Literal)sender).Text = string.Format("<a href='{0}?f={1}/{2}/{3}' ><img src='{4}' border='0' /></a>", page.ResolveUrl("~/lib/download.ashx"), "~/upload/lesson/", page.Request.QueryString["ID"], row[CN], page.ResolveUrl("~/assistportal/images/detail.gif"));
    //            };

    //            tf.ItemTemplate = li;
    //            //                    tf.ItemStyle.Width = 80;
    //            gv.Columns.Add(tf);
    //        }
    //        else
    //        {
    //            BoundField bf = new BoundField();
    //            bf.HeaderText = dt.Columns[i].Caption;
    //            bf.DataField = dt.Columns[i].ColumnName;
    //            //                    bf.ItemStyle.Width = 80;
    //            gv.Columns.Add(bf);
    //        }
    //    }
    //}

    public DataView GetQty(string LessonID, string RndID)
    {
        ListItemCollection param = new ListItemCollection();
        string sql = @"select isnull(ClsQty,99999)ClsQty,isnull(b.Qty,0) Qty from LessonRnd a
left join (select LessonID,COUNT(distinct MemberNo) Qty  from LessonRegister where RndID = @RndID  group by LessonID) b
on a.LessonID = b.LessonID
where  a.LessonID = @LessonID and a.RndID = @RndID";
        param.Add(new ListItem("LessonID", LessonID));
        param.Add(new ListItem("RndID", RndID));
        DataView dv = this.selectSQL(sql, param);

        return dv;
    }

    /// <summary>
    /// 報名審核通過
    /// </summary>
    /// <param name="LessonID"></param>
    /// <returns>Dictionary<Email,MemberName></returns>
    public Dictionary<string, string> QualifiedList(string LessonID)
    {
        string[] P = LessonID.Split('_');
        ListItemCollection p = new ListItemCollection();
        p.Add(new ListItem("LessonID", P[0])); //0
        p.Add(new ListItem("RndID", P[1])); //1

        DataView dv0 = selectSQL(@"
 select b.Email,b.MemberName from LessonRegister a inner join Member b on a.MemberNo = b.MemberNo inner join LessonRnd c on a.RndID= c.RndID and a.LessonID = c.LessonID
 where a.Status =1 and a.LessonID=@LessonID and a.RndID=@RndID

", p);

        Dictionary<string, string> dic = new Dictionary<string, string>();
        for (int i = 0; i < dv0.Count; i++)
        {
            if (!dic.ContainsKey(dv0[i]["Email"].ToString()))
                dic.Add(dv0[i]["Email"].ToString(), dv0[i]["MemberName"].ToString());
        }

        return dic;
    }

    public string CancelLesson(string LessonID, string status,  string MemberNo="" ,string cancelTime= "")
    {  //status 1:報到 ,4 :退訓 9:未到
        if (cancelTime == string.Empty)
            cancelTime = DateTime.Now.ToString("yyyy/MM/dd");

        ListItemCollection param = new ListItemCollection();
        string sql1 = @"UPDATE LessonRegister SET Status = @status, TotalStatus='4' ,CancelTime=@CancelTime WHERE LessonID = @LessonID AND MemberNo = @MemberNo";
        param.Add(new ListItem("LessonID", LessonID));
        param.Add(new ListItem("CancelTime", cancelTime));
        param.Add(new ListItem("status", status));
        param.Add(new ListItem("MemberNo", MemberNo == "" ? MyApp.Member.Accnt : MemberNo));

        DataView qt = this.selectSQL(@"select Count(Distinct b.MemberNo)Qty, isnull(a.ClsQty, 999999999)ClsQty from Lesson a inner join LessonRegister b
on a.LessonID=b.LessonID where b.Status=1 and a.LessonID=@LessonID group by a.ClsQty", param);

        if (qt.Count > 0)
        {
            if (int.Parse(qt[0]["ClsQty"].ToString()) >= int.Parse(qt[0]["Qty"].ToString()))
            {
//                this.instSQL(@"update LessonRegister set LessonRegister.isZhengqu=1 from
//                    (select Min(RegistDate) min_aID, aID from LessonRegister where isZhengqu=0 and Status=0 gruop by aID)b
//                    where b.aID=LessonRegister.aID");
            }
        }

        int i = new myDB().updSQL(sql1, param);

        //string sql2 = @"Delete MyLessonClosedScores WHERE LessonID = @LessonID AND MemberNo = @MemberNo";

        //int j = new myDB().delSQL(sql2, param);

        if (i > 0) { return "1"; } else { return "0"; }
    }

    public string ApprovedLesson(string LessonID, string MemberNo = "")
    {
        ListItemCollection param = new ListItemCollection();
        string sql1 = @"UPDATE LessonRegister SET Status = '1', TotalStatus='1',CancelTime=null WHERE LessonID = @LessonID AND MemberNo = @MemberNo";
        param.Add(new ListItem("LessonID", LessonID));
        param.Add(new ListItem("MemberNo", MemberNo == "" ? MyApp.Member.Accnt : MemberNo));

//        DataView qt = this.selectSQL(@"select Count(Distinct b.MemberNo)Qty, isnull(a.ClsQty, 999999999)ClsQty from Lesson a inner join LessonRegister b
//        on a.LessonID=b.LessonID where b.Status=1 and a.LessonID=@LessonID group by a.ClsQty", param);

//        if (qt.Count > 0)
//        {
//            if (int.Parse(qt[0]["ClsQty"].ToString()) >= int.Parse(qt[0]["Qty"].ToString()))
//            {
//                this.instSQL(@"update LessonRegister set LessonRegister.isZhengqu=1 from
//                            (select Min(RegistDate) min_aID, aID from LessonRegister where isZhengqu=0 and Status=0 gruop by aID)b
//                            where b.aID=LessonRegister.aID");
//            }
//        }

        int i = new myDB().updSQL(sql1, param);

        //string sql2 = @"Delete MyLessonClosedScores WHERE LessonID = @LessonID AND MemberNo = @MemberNo";

        //int j = new myDB().delSQL(sql2, param);

        if (i > 0) { return "1"; } else { return "0"; }
    }


    public DataView CheckLessonRIsExist(string key)
    {
        var sql = "select * from LessonR where LessonRName= @LessonRName";
        var r = this.selectSQL(sql, new ListItemCollection
        {
            new ListItem("LessonRName",key)
        });

        return r;
    }

    public string GetStatus(string aid)
    {
        var sql = "select  status from Lesson where aid=@aid";
        var dv = this.selectSQL(sql, new ListItemCollection
        {
            new ListItem("aid",aid)
        });

        return dv != null && dv.Count > 0 ? dv[0]["status"].ToString() : "0"; //0 才可刪除
    }

    /// <summary>
    /// 取得學員當天的出席紀錄 包含架別
    /// </summary>
    /// <param name="rndid"></param>
    /// <param name="lessonid"></param>
    /// <returns></returns>
    public DataView GetLessonAttendSetHoliday(string date0, string lessonid)
    {
        var sql = @"

SELECT       las.IsAttend,  las.LSettingID, las.HolidayID, las.MemberNo, lr.Date0, las.LessonID,las.Memo
FROM              LessonAttendSet AS las INNER JOIN
                 LessonRnd AS lr ON las.RndID = lr.RndID AND las.LanNo = lr.LanNo AND las.LessonID = lr.LessonID
where lr.LessonID=@LessonID and Convert(char(10),lr.Date0,111)=@Date0 and lr.LanNo=@LanNo;
";

        var dv = this.selectSQL(sql, new ListItemCollection
        {
            new ListItem("Date0",date0),
            new ListItem("LessonID",lessonid),
            new ListItem("LanNo",tool.currentLanguage)
        });

        return dv;
    }


    public LessonsCalendar CallBackData_RndOfDay(string yy, string mm)
    {
        ListItemCollection _params = new ListItemCollection();
        _params.Add(new ListItem("yy", (int.Parse(yy) + 1911).ToString()));
        _params.Add(new ListItem("mm", mm));
//        DataView dv = selectSQL(@"select  c.counts,a.LessonName,b.EDate, Day(b.EDate)_Day from LessonPlan a
//left join LessonPlanEvent b on a.PlanID = b.PlanID
//left join (select Count(PlanID)counts, EDate from LessonPlanEvent group by EDate) c on b.EDate=c.EDate
//where Year(b.EDate)=@yy and Month(b.EDate)=@mm order by EDate", _params);

        LessonsCalendar calendar = new LessonsCalendar
        { 
            LessonCalendar = selectSQL(@"select Count(*)counts, * from(
                select Convert(char(10), RDate0, 120) EDate,LessonID, LessonName, Day(RDate0) _Day, 1 DateType from Lesson
                where Year(RDate0)=@yy and Month(RDate0)=@mm and OpenShow=1
                union all
                select Convert(char(10), RDate1, 120) EDate, LessonID,LessonName, Day(RDate1) _Day, 2 DateType from Lesson
                where Year(RDate1)=@yy and Month(RDate1)=@mm and OpenShow=1
                ) t group by EDate, _Day, LessonID,LessonName, DateType", _params).ToTable(),
            NoLessonCalendar = selectSQL(@"select Count(*)counts, * from(
                select Convert(char(10), RDate0, 120) EDate, Day(RDate0) _Day, 1 DateType from Lesson
                where Year(RDate0)=@yy and Month(RDate0)=@mm and OpenShow=1
                union all
                select Convert(char(10), RDate1, 120) EDate, Day(RDate1) _Day, 2 DateType from Lesson
                where Year(RDate1)=@yy and Month(RDate1)=@mm and OpenShow=1
                ) t group by EDate, _Day, DateType", _params).ToTable()
        };
        return calendar;
    }

    public void UpdatePayDanID(string aid, string paydanID)
    {
        this.updSQL("update lesson set PayDanID=@PayDanID where aID=@aID",
            new ListItemCollection
            {
                new ListItem("PayDanID",paydanID),
                new ListItem("aID",aid)
            });
    }

    public bool CheckLearnHourEditExist(string umember, string lessonid)
    {
        var dv = this.selectSQL("select aid from LessonLearnHourEdit where LessonID=@LessonID and UMember = @UMember"
            , new ListItemCollection { new ListItem("LessonID", lessonid), new ListItem("UMember", umember) });
        return dv.Count > 0;
    }

    public List<SendMailList> LessonStartNotice(string LessonID)
    {
        string[] P = LessonID.Split('_');
        ListItemCollection p = new ListItemCollection();
        p.Add(new ListItem("LessonID", P[0])); //0

        DataView dv0 = selectSQL(@"
 select b.Email,b.MemberName,b.UMember,a.MemberNo from LessonRegister a
 inner join Member b on a.MemberNo = b.MemberNo
 inner join LessonRnd c on  a.LessonID = c.LessonID
 where a.Status =1 and a.LessonID=@LessonID
", p);

        var userNo = new List<SendMailList>();
        for (int i = 0; i < dv0.Count; i++)
        {
            if (userNo.FirstOrDefault(x => x.Email == dv0[i]["Email"].ToString()) == null)
            {
                userNo.Add(new SendMailList
                {
                    Email = dv0[i]["Email"].ToString(),
                    Name = dv0[i]["MemberName"].ToString(),
                    UserNo = dv0[i]["MemberNo"].ToString()
                });
            }
        }

        return userNo;
    }

    public List<SendMailList> LessonStartNoticeForTeacher(string LessonID)
    {
        string[] P = LessonID.Split('_');
        ListItemCollection p = new ListItemCollection();
        p.Add(new ListItem("LessonID", P[0])); //0

        DataView dv0 = selectSQL(@"

SELECT  distinct   l.LessonID, l.LessonName, l.RDate0, el.Email, el.LecturerName, el.LecturerNo,el.Account
FROM      Lesson AS l INNER JOIN
                   LessonRnd AS lrnd ON l.LanNo = lrnd.LanNo AND l.LessonID = lrnd.LessonID INNER JOIN
                   LessonLecturerGroup AS llg ON lrnd.RndID=llg.RndID and lrnd.LessonID=llg.LessonID INNER JOIN
                   EducationLecturer AS el ON llg.LanNo = el.LanNo AND llg.LecturerId = el.LecturerId
WHERE   (l.LessonID = @LessonID)
", p);

        var userNo =new List<SendMailList>();
        for (int i = 0; i < dv0.Count; i++)
        {
            if (userNo.FirstOrDefault(x=>x.Email == dv0[i]["Email"].ToString()) == null)
            {
                userNo.Add(new SendMailList 
                {
                    Email = dv0[i]["Email"].ToString(),
                    Name = dv0[i]["LecturerName"].ToString(),
                    UserNo = dv0[i]["LecturerNo"].ToString()
                });
            }
        }

        return userNo;
    }

    /// <summary>
    /// 取得配當表的課程
    /// </summary>
    /// <returns></returns>
    public string GetPanDanLessonAutoData(string lessonid)
    {
        var dv = this.selectSQL(@"
Select * from (
SELECT         pdd.LessonRID,  lr.LessonRName,lr.LessonPro
FROM              PayDan AS pd INNER JOIN
                            PayDanDetail AS pdd ON pd.PayDanID = pdd.PayDanID INNER JOIN
                            Lesson AS l ON pd.PayDanID = l.PayDanID INNER JOIN
                            LessonR AS lr ON pdd.LessonRID = lr.LessonRID
Where LessonID=@LessonID
union
SELECT  LessonRID, LessonRName,LessonPro
FROM      LessonR
where LessonPro = 2 --一般課程
) t  order by LessonPro
", new ListItemCollection
 {
     new ListItem("LessonID",lessonid)
 });

        var result = new List<PanDanModel>();
        if (dv != null && dv.Count > 0)
        {
            for (int i = 0; i < dv.Count; i++)
                result.Add(new PanDanModel
                {
                    label = string.Format("{0}", dv[i]["LessonRName"].ToString()),
                    value = string.Format("{0}", dv[i]["LessonRID"].ToString())
                });

            //return string.Format("[{0}]",string.Join(",",result));
            return JsonConvert.SerializeObject(result);
        }

        return "";
    }

    public bool HasNoticeAddMember(string lessonID)
    {
        var dv = this.selectSQL("select NoticeAddMember from Lesson where LessonID=@LessonID"
           , new ListItemCollection { new ListItem("LessonID", lessonID) });

        return dv.Count > 0 && dv[0][0] != System.DBNull.Value ? System.Convert.ToBoolean(dv[0][0]) : false;
    }

    public bool HasNoticeAddGroupRole(string lessonID)
    {
        var dv = this.selectSQL("select NoticeAddGroupRole from Lesson where LessonID=@LessonID"
           , new ListItemCollection { new ListItem("LessonID", lessonID) });

        return dv.Count > 0 && dv[0][0] != System.DBNull.Value ? System.Convert.ToBoolean(dv[0][0]) : false;
    }

    public DataView GetLessonData(string lessonid)
    {
        var dv = this.selectSQL(@"
SELECT  pd.PayDanName, l.*, ea.AddressName
FROM      Lesson AS l left JOIN
                   PayDan AS pd ON l.LanNo = pd.LanNo AND l.PayDanID = pd.PayDanID INNER JOIN
                   EducationAddress AS ea ON l.DefaultRoom = ea.aID
where l.LessonID=@LessonID"
   , new ListItemCollection { new ListItem("LessonID", lessonid) });

        return dv;
    }

    public DataView SelectLessonByDate(string date)
    {
        return this.selectSQL("select RndID,LessonID,LessonRID from LessonRnd where date0=@date"
           , new ListItemCollection{new ListItem("date",date)});
        
    }

    public void SaveLessonScoreTotal(DataView dvScore, string lessonid)
    {
        var trans = new SqlTransHelper();
        trans.CreateCommand("delete from LessonScoreTotal where LessonID=@LessonID"
            , new SqlTransParameter("LessonID", lessonid));

        for (int i = 0; i < dvScore.Count; i++)
        {
            trans.CreateCommand(@"insert into LessonScoreTotal (LessonID,MemberNo,ExScore1,ExScore2,TotalExScore,CrtUser)
values(@LessonID,@MemberNo,@ExScore1,@ExScore2,@TotalExScore,@CrtUser)"
             , new SqlTransParameter("LessonID", lessonid)
             , new SqlTransParameter("MemberNo", dvScore[i]["MemberNo"].ToString())
             , new SqlTransParameter("ExScore1", dvScore[i]["ExScore1"].ToString() == "" ? "0" : dvScore[i]["ExScore1"].ToString())
             , new SqlTransParameter("ExScore2", dvScore[i]["ExScore2"].ToString() == "" ? "0" : dvScore[i]["ExScore2"].ToString())
             , new SqlTransParameter("TotalExScore", dvScore[i]["TotalExScore"].ToString() == "" ? "0" : dvScore[i]["TotalExScore"].ToString())
             , new SqlTransParameter("CrtUser", MyApp.SysUser.Accnt)
             );

            this.ExecuteTranscation(trans);
         
        }

    }

}


public class PanDanModel
{
    public string label { set; get; }
    public string value { set; get; }
}

public class LessonsCalendar
{
    public DataTable LessonCalendar { get; set; }
    public DataTable NoLessonCalendar { get; set; }
}

public class SendMailList
{
    public string Email { set; get; }

    public string Name { set; get; }

    public string UserNo { set; get; }


}