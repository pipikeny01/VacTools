using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacToolsTests.Stub;

namespace VacToolsTests.Factory
{
    public class MockFactorys
    {
        private static string _connString = "server=192.168.1.166;user id=netuser;password=gigi0913;persistsecurityinfo=True;database=vac_elearning;Allow User Variables=True";

        public static clsLesson GetClsLessonStub()
        {
            var mydb = new clsLessonStub();
            mydb.SetConnectString(_connString);
            return mydb;
        }

        public static clsLessonSetting GetClsLessonSettingStub()
        {
            var mydb = new clsLessonSettingStub();
            mydb.SetConnectString(_connString);
            return mydb;
        }

        public static clsLessonRnd GetClsLessonRndStub()
        {
            var mydb = new clsLessonRndStub();
            mydb.SetConnectString(_connString);
            return mydb;
        }

        public static clsEducationLecturerStub GetClsEducationLecturerStub()
        {
            var mydb = new clsEducationLecturerStub();
            mydb.SetConnectString(_connString);
            return mydb;
        }


    }
}