using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IclsPayDanLecturer
{
    DataTable SelectPayDanLecturerDataTable(string paydanID);

    clsPayDanLecturer.AddStatus Add(string payDanID, string payDetailID, string teacherID, string user);
}
