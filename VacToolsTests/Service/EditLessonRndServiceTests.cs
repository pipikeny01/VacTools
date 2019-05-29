using Microsoft.VisualStudio.TestTools.UnitTesting;
using VacWebSiteTools.Service;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NSubstitute;
using VacWebSiteTools.Helper;

namespace VacWebSiteTools.Service.Tests
{
    [TestClass()]
    public class EditLessonRndServiceTests
    {
        private IclsParameters mockParameters;
        private IclsEducationLecturer mockEducationLecturer;
        private ITeacherQualifi _teacherQualifi;
        private EditLessonRndService _service;

        [TestInitialize]
        public void Initial()
        {
            mockParameters = Substitute.For<IclsParameters>();
            mockEducationLecturer = Substitute.For<IclsEducationLecturer>();
            _teacherQualifi = new TeacherQualifi(mockParameters);

            mockParameters.TeacherQualityualifi.Returns(new clsParameters.TeacherQualityualifiODT
            {
                Age_limit = 65,
                Iteacher_hour_limit = 80,
                Oteacher_hour_limit = 60
            });

            _service = new EditLessonRndService(_teacherQualifi, mockEducationLecturer);
        }

        [TestMethod()]
        public void CheckQualifiTest_年齡66_應該回傳OverYearsOld()
        {
            var teacherYearsOld = 66;
            var teacherKind = "A";
            var teacHour = 85;

            mockEducationLecturer.SelectTeacherHourCount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(SelectTeacherHourCountDataView(teacherYearsOld, teacherKind, teacHour));

            var expect = CheckQualifiResult.OverYearsOld;
            var result = _service.CheckQualifi("", "", "");
            Assert.AreEqual(expect, result);
        }

        [TestMethod()]
        public void CheckQualifiTest_年齡40_講師類型內聘A_時數85應該回傳InOverHour()
        {
            var teacherYearsOld = 40;
            var teacherKind = "A";
            var teacHour = 85;

            mockEducationLecturer.SelectTeacherHourCount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(SelectTeacherHourCountDataView(teacherYearsOld, teacherKind, teacHour));

            var expect = CheckQualifiResult.InOverHour;
            var result = _service.CheckQualifi("", "", "");
            Assert.AreEqual(expect, result);
        }

        [TestMethod()]
        public void CheckQualifiTest_年齡40_講師類型內聘A_時數80應該回傳NoProblem()
        {
            var teacherYearsOld = 40;
            var teacherKind = "A";
            var teacHour = 80;

            mockEducationLecturer.SelectTeacherHourCount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(SelectTeacherHourCountDataView(teacherYearsOld, teacherKind, teacHour));

            var expect = CheckQualifiResult.NoProblem;
            var result = _service.CheckQualifi("", "", "");
            Assert.AreEqual(expect, result);
        }

        [TestMethod()]
        public void CheckQualifiTest_年齡40_講師類型外聘B_時數61應該回傳OutOverHour()
        {
            var teacherYearsOld = 40;
            var teacherKind = "B";
            var teacHour = 61;

            mockEducationLecturer.SelectTeacherHourCount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(SelectTeacherHourCountDataView(teacherYearsOld, teacherKind, teacHour));

            var expect = CheckQualifiResult.OutOverHour;
            var result = _service.CheckQualifi("", "", "");
            Assert.AreEqual(expect, result);
        }

        [TestMethod()]
        public void CheckQualifiTest_年齡40_講師類型外聘B_時數60應該回傳NoProblem()
        {
            var teacherYearsOld = 40;
            var teacherKind = "B";
            var teacHour = 60;

            mockEducationLecturer.SelectTeacherHourCount(Arg.Any<string>(), Arg.Any<string>(), Arg.Any<string>())
                .Returns(SelectTeacherHourCountDataView(teacherYearsOld, teacherKind, teacHour));

            var expect = CheckQualifiResult.NoProblem;
            var result = _service.CheckQualifi("", "", "");
            Assert.AreEqual(expect, result);
        }

        private DataView SelectTeacherHourCountDataView(int teacherYearsOld, string teacherKind, int teacHour)
        {
            var dt = new DataTable();
            dt.Columns.Add("yearsold");
            dt.Columns.Add("TeacherKind");
            dt.Columns.Add("HourCount");
            var newRow = dt.NewRow();
            newRow["yearsold"] = teacherYearsOld;
            newRow["TeacherKind"] = teacherKind;
            newRow["HourCount"] = teacHour;
            dt.Rows.Add(newRow);
            return dt.DefaultView;
        }
    }
}