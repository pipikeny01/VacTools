using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacWebSiteTools;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class AgeTests
    {
        [TestMethod()]
        public void CalculateAgeTest_測試傳入19810708及20190415日期回傳37年()
        {
            var ageHelper = new Age();
            var start = new DateTime(1981, 7, 8);
            var end = new DateTime(2019, 4, 15);
            var expected = 37;
            var result = ageHelper.CalculateAge(start, end);
            Assert.AreEqual(expected, result);
        }

    }
}
