using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using aiet.Base;
using Aiet_DB;
using MySql.Data.MySqlClient;

public class clsEducationLecRetire:BaseTable
{
    public clsEducationLecRetire()
    {
        this.TableName = "education_lec_retire";
        NameField = "Display";
        pk = "aID";
        dfs = new DataFields("aID", "LevelText");

    }

    public void UpdateMaxAmount(dynamic updateDatasList)
    {
        var trans = new SqlTransHelper();
        foreach (var o in updateDatasList)
        {
            trans.CreateCommand("update education_lec_retire set MaxAmount= @MaxAmount where IdentityNo=@IdentityNo"
                , new SqlTransParameter("MaxAmount", o.MaxAmount)
                , new SqlTransParameter("IdentityNo", o.IdentityNo));

        }

        this.ExecuteTranscation(trans);
    }
}
