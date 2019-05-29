using Microsoft.VisualStudio.TestTools.UnitTesting;
using VacWebSiteTools.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools.Service.Tests
{
    [TestClass()]
    public class LessonCopyNewDateHandlerTests
    {
        [TestMethod()]
        public void NextDateTest多個參數的變動結果是否如預期()
        {
            var handler = new LessonCopyNewDateHandler(new DateTime(2019, 2, 1));
            Assert.AreEqual(new DateTime(2019, 2, 1), handler.GetNewCopyDate(new DateTime(2019, 1, 17)));
            Assert.AreEqual(new DateTime(2019, 2, 1), handler.GetNewCopyDate(new DateTime(2019, 1, 17)));
            Assert.AreEqual(new DateTime(2019, 2, 4), handler.GetNewCopyDate(new DateTime(2019, 1, 18)));
            Assert.AreEqual(new DateTime(2019, 2, 4), handler.GetNewCopyDate(new DateTime(2019, 1, 18)));
            Assert.AreEqual(new DateTime(2019, 2, 5), handler.GetNewCopyDate(new DateTime(2019, 1, 21)));
        }
    }
}