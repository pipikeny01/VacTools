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
public class clsLessonR : BaseTable
{

    DataView listName;
    ListItemCollection _params = new ListItemCollection();
    private DataView dv = null;

    public static List<ListItem> LessonRItems
    {

        get
        {
            return new List<ListItem>
            {
                new ListItem("",""),
                new ListItem("專業課目","1"),
                new ListItem("一般課目","2")
            };


        }

    }
    public clsLessonR()
    {
        this.TableName = "LessonR";
        NameField = "LessonRName";
        pk = "LessonRID";
        dfs = new DataFields("LessonRID", "LessonRName");
    }
    public override string getSNo
    {
        get
        {
            return new clsSerialFmt().getSNo("LessonR", 10);
        }
    }

    public bool isExisted(string Lang, string LessonRID)
    {
        selectText = @"select aID from LessonR where LessonRID=@LessonRID and LanNo=@LanNo";
        _params.Clear();
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        _params.Add(new ListItem("LessonRID", LessonRID));
        return selectSQL(_params).Count == 0 ? false : true;
        

    }

    public bool isExisted(string aID, string Lang, string LessonRID)
    {
        selectText = @"select aID from LessonR where LessonRID=@LessonRID and LanNo=@LanNo and aID <> @ID";
        _params.Clear();
        _params.Add(new ListItem("ID", aID));
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        _params.Add(new ListItem("LessonRID", LessonRID));
        return selectSQL(_params).Count == 0 ? false : true;

    }

    public override ListItem[] Items
    {
        get
        {
            ItemsWhereString = " and LanNo=@LanNo";
            ItemsParams.Add(new ListItem("LanNo", tool.currentLanguage));
            return base.Items;
        }
    }

    public override bool isUsed(string id)
    {
        selectText = @"select  b.aID from LessonR a inner join LessonRnd b
  on a.LessonRID = b.LessonRID
  where b.aID =@ID";
        _params.Clear();
        _params.Add(new ListItem("ID", id));
        return selectSQL(_params).Count > 0;
    }

    public ListItem[] LessonRKindList()
    {
        List<ListItem> ClassList = new List<ListItem>();
        if (ClassList.Count == 0)
        {

            dv = selectSQL(@"select LessonRKindID, LessonRKindName from LessonRKind");

        }
        ClassList.Add(new ListItem("", ""));
        for (int i = 0; i < dv.Count; i++)
        {
            ClassList.Add(new ListItem(dv[i]["LessonRKindName"].ToString(), dv[i]["LessonRKindID"].ToString()));
        }
        return ClassList.ToArray();
    }

    public string LessonRKindText(string s)
    {
        ListItem JobTitle;
        if (string.IsNullOrEmpty(s))
            return "";
        else
        {
            JobTitle = LessonRKindList().FirstOrDefault(p => p.Value == s);

            return JobTitle == null ? "" : JobTitle.Text;
        }
    }

    public ListItem[] LessonRTypeList()
    {
        List<ListItem> ClassList = new List<ListItem>();
        ClassList.Add(new ListItem("", ""));
        ClassList.Add(new ListItem("主科", "1"));
        ClassList.Add(new ListItem("副科", "2"));
        ClassList.Add(new ListItem("不測驗", "3"));
        return ClassList.ToArray();
    }

    public string LessonRTypeText(string s)
    {
        ListItem LessonRType;
        if (string.IsNullOrEmpty(s))
            return "";
        else
        {
            LessonRType = LessonRTypeList().FirstOrDefault(p => p.Value == s);

            return LessonRType == null ? "" : LessonRType.Text;
        }
    }


    public DataView CheckLessonRIsExist(string name)
    {
        var sql = "select * from LessonR where LessonRName= @LessonRName";
        var r = this.selectSQL(sql, new ListItemCollection 
        {
            new ListItem("LessonRName",name)
        });

        return r;
    }

}
