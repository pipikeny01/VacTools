using Microsoft.VisualStudio.TestTools.UnitTesting;
using VacWebSiteTools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacToolsTests.Stub;
using Rhino.Mocks;
using NSubstitute;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class PayDanHelperTests
    {
        [TestMethod()]
        public void GetPayDanTitlesTest_呼叫帶入日期20180117和20180704取得第二周是20180121到20180127_自訂stub()
        {
            //Arrange
            var payDanHelper = new PayDanHelper(new DateHelperStub());
            var startDate = new DateTime(2018, 1, 17);
            var endDate = new DateTime(2018, 7, 4);
            var expectedstartDate = new DateTime(2018, 1, 21);
            var expectedEndDate = new DateTime(2018, 1, 27);
            //Act
            var result = payDanHelper.GetPayDanWeeks(startDate, endDate);
            //Assert
            Assert.AreEqual(expectedstartDate, result[1].StarDate);
            Assert.AreEqual(expectedEndDate, result[1].EndDate);

        }


        [TestMethod()]
        public void GetPayDanTitlesTest_呼叫帶入日期20180117和20180704取得集合是25個() //其實這個也不用測了
        {
            //使用mock framework    Rhino.Mocks
            //Arrange
            //建立一個mock物件
            var stubDateHelper = MockRepository.GenerateStub<IDateHelper>(); 
            
            var payDanHelper = new PayDanHelper(stubDateHelper);

            var startDate = new DateTime(2018, 1, 17);
            var endDate = new DateTime(2018, 7, 4);
            var expectedListCount = 25;

            //mock 使用要使用的方法 , 並回傳值 , 這時候帶入的參數是多少不重要 ,因為mock物件本身根本沒有邏輯運算
            stubDateHelper.Stub(p => p.GetTotalWeekCount(startDate, endDate)).Return(25);
            stubDateHelper.Stub(p => p.GetWeekOfDate(startDate)).Return(3);

            //Act
            var result = payDanHelper.GetPayDanWeeks(startDate,endDate);
            //Assert
            Assert.AreEqual(expectedListCount, result.Count);

        }
        
        [TestMethod()]
        public void GetPayDanTitlesTest_呼叫帶入日期20180117和20180704取得第二周是20180121到20180127_NSubstitute()
        {
            //使用mock framework  NSubstitute
            //Arrange
            //建立一個mock物件
            var mock = Substitute.For<IDateHelper>();
            var payDanHelper = new PayDanHelper(mock);   
            var startDate = new DateTime(2018, 1, 17);
            var endDate = new DateTime(2018, 7, 4);
            var expectedstartDate = new DateTime(2018, 1, 21);
            var expectedEndDate = new DateTime(2018, 7, 4);

            mock.GetTotalWeekCount(startDate, endDate).Returns(25);
            mock.GetWeekOfDate(startDate).Returns(3);

            //Act
            var result = payDanHelper.GetPayDanWeeks(startDate, endDate);

            //Assert
            Assert.AreEqual(expectedstartDate, result[1].StarDate);
            Assert.AreEqual(expectedEndDate, result[24].EndDate);

        }


    }
}