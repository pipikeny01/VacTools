using Microsoft.VisualStudio.TestTools.UnitTesting;
using VacWebSiteTools.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacToolsTests.Factory;

namespace VacWebSiteTools.Service.Tests
{
    [TestClass()]
    public class LessonServiceTests
    {
        [TestInitialize]
        public void Initial()
        {
            MyApp.ForTesting = true;
        }

        [TestMethod()]
        public void LessonCopyTest整合測試班級複製功能()
        {
            var db =MockFactorys.GetClsLessonStub();
            var dv = db.getRowById("1088501007");
            var lesson = new LessonService(MockFactorys.GetClsLessonStub()
                ,MockFactorys.GetClsLessonSettingStub()
                ,MockFactorys.GetClsLessonRndStub());
            var expect = "複製已完成";
            var result =  lesson.LessonCopy(dv, "1088501007", "108/02/01");

            Assert.AreEqual(expect, result);
        }

        [TestCleanup]
        public void CleanUp()
        {
            MyApp.ForTesting = false;

        }
    }
}