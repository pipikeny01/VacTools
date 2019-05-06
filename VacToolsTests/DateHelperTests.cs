using Microsoft.VisualStudio.TestTools.UnitTesting;
using VacWebSiteTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class DateHelperTests
    {
        [TestMethod()]
        public void GetTotalWeekCountTest_測試20180117到20180704區間會有25周()
        {
            //Arrange
            var dateHelper = new DateHelper();
            var sdate = new DateTime(2018, 1, 17);
            var edate = new DateTime(2018, 7, 4);
            var expected = 25;
            //Act
            var result = dateHelper.GetTotalWeekCount(sdate, edate);
            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetTotalWeekCountTest_測試20190101到20190415區間會有16周()
        {
            //Arrange
            var dateHelper = new DateHelper();
            var sdate = new DateTime(2019, 1, 1);
            var edate = new DateTime(2019, 4, 15);
            var expected = 16;
            //Act
            var result = dateHelper.GetTotalWeekCount(sdate, edate);
            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetWeekOfDateTest_測試帶入201801017日期要回傳星期3()
        {
            //Arrange
            var dateHelper = new DateHelper();
            var date = new DateTime(2018, 1, 17);
            var expected = 3;
            //Act
            var result = dateHelper.GetWeekOfDate(date);
            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void GetWeekOfDateTest_測試帶入201803004日期應該要回傳星期0()
        {
            //Arrange
            var dateHelper = new DateHelper();
            var date = new DateTime(2018, 3, 4);
            var expected = 0;
            //Act
            var result = dateHelper.GetWeekOfDate(date);
            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void WeekOfYearTest_呼叫取得20180117是該年的第3周()
        {
            //Arrange
            var dateHelper = new DateHelper();
            var date = new DateTime(2018, 1, 17);
            var expected = 3;
            //Act
            var result = dateHelper.GetWeekOfYearFromDate(date);
            //Assert
            Assert.AreEqual(expected, result);
        }

        [TestMethod()]
        public void WeekOfYearTest_呼叫取得20180704是該年的第27周()
        {
            //Arrange
            var dateHelper = new DateHelper();
            var date = new DateTime(2018, 7, 4);
            var expected = 27;
            //Act
            var result = dateHelper.GetWeekOfYearFromDate(date);
            //Assert
            Assert.AreEqual(expected, result);
        }

    }
}