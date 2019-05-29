using aiet.Base;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;

public class clsLessonRnd:BaseTable
{
    public clsLessonRnd()
    {
        this.TableName = "LessonRnd";
        NameField = "";
        pk = "aID";

    }

    public DataView SelectDistinctForLessonService(string copyLessonID)
    {
        var dvLrnd_Count = this.selectSQL("select distinct LessonID, RndID from LessonRnd where LessonID=@LessonID order by Date0"
          , new ListItemCollection { new ListItem("LessonID", copyLessonID) });

        return dvLrnd_Count;
    }

    public DataView GetRndDate(string copyLessonID , string rndID)
    {
        var dvLrnd = this.selectSQL("select * from LessonRnd where LessonID=@LessonID and RndID=@RndID order by Date0"
, new ListItemCollection {
                    new ListItem("LessonID", copyLessonID),
                    new ListItem("RndID", rndID)
});

        return dvLrnd;
    }
}
