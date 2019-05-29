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
public class clsLessonRKind : BaseTable
{

    DataView listName;
    ListItemCollection _params = new ListItemCollection();

    public clsLessonRKind()
    {
        this.TableName = "LessonRKind"; //資料表
        NameField = "LessonRKindName"; // 要顯示出來的欄位,例如 Dropdowlist(如不需要可不填)
        pk = "LessonRKindID"; //真的主鍵欄位
        dfs = new DataFields(pk, NameField);
    }

    public bool isExisted(string Lang, string LessonRKindID)
    {
        selectText = @"select aID from LessonRKind where LessonRKindID=@LessonRKindID and LanNo=@LanNo";
        _params.Clear();
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        _params.Add(new ListItem("LessonRKindID", LessonRKindID));
        return selectSQL(_params).Count == 0 ? false : true;
        

    }

    public bool isExisted(string aID, string Lang, string LessonRKindID)
    {
        selectText = @"select aID from LessonRKind where LessonRKindID=@LessonRKindID and LanNo=@LanNo and aID <> @ID";
        _params.Clear();
        _params.Add(new ListItem("ID", aID));
        _params.Add(new ListItem("LanNo", tool.currentLanguage));
        _params.Add(new ListItem("LessonRKindID", LessonRKindID));
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
        selectText = @"select  b.aID from LessonR a inner join LessonRKind b
  on a.LessonRKindID = b.LessonRKindID
  where b.aID =@ID";
        _params.Clear();
        _params.Add(new ListItem("ID", id));
        return selectSQL(_params).Count > 0;
    }

    public override string getSNo
    {
        get
        {
            return new clsSerialFmt().getSNo("LessonRKind", 4);
        }
    }


    public DataView CheckLessonRKindIsExist(string name)
    {
        var sql = "select * from LessonRKind where LessonRKindName= @LessonRKindName";
        var r = this.selectSQL(sql, new ListItemCollection 
        {
            new ListItem("LessonRKindName",name)
        });

        return r;
    }
}
