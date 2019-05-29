using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacToolsTests.Stub
{
    public class clsLessonStub : clsLesson
    {
        public override string getSNo
        {
            get
            {
                return "2000000002";
            }
        }
    }

    public class clsLessonSettingStub : clsLessonSetting
    {
    }

    public class clsLessonRndStub : clsLessonRnd
    {
    }

    public class clsEducationLecturerStub : clsEducationLecturer
    {

    }

}
