using aiet.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

public class clsEducation_level : BaseTable
{
    public clsEducation_level()
    {
        this.TableName = "education_level";
        NameField = "LevelText";
        pk = "aID";
        dfs = new DataFields("aID", "LevelText");

    }
}

