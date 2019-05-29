using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacWebSiteTools.Helper;

namespace VacWebSiteTools
{
    public class RepositoryFactory
    {
        public static myDB GetMyDB()
        {
            return new myDB();
        }
        public static clsEducation_level GetClsEducation_level()
        {
            return new clsEducation_level();
        }

        public static clsPayDanDetail GetClsPayDanDetail()
        {
            return new clsPayDanDetail();
        }

        public static clsParameters GetClsParameters()
        {
           return  new clsParameters();
        }

        public static clsPayDanLecturer GetClsPayDanLecturer()
        {
            return  new clsPayDanLecturer();
        }

        public static Age GetAge()
        {
            return new Age();
        }

        public static clsPayDan GetClsPayDan()
        {
            return new clsPayDan();
        }

        public static ImportPayDanHelper GetImportPayDanHelper()
        {
            return new ImportPayDanHelper();
        }

        public static clsEducationLecturer GetClsEducationLecturer()
        {
            return new clsEducationLecturer();
        }

        public static TeacherQualifi GetTeacherQualifi()
        {
            return new TeacherQualifi();
        }

    }
}
