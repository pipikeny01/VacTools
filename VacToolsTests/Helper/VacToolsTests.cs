using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class VacToolsTests
    {
        [TestMethod()]
        public void Get_代入匯入時講師名稱_逗號分隔取得3個教師_第一個是李xx()
        {
            //Arrange
            var param = "李xx,陳某某,黃娟娟";
            var expectedFirstElm = "李xx";
            var expectedCount = 3;
            //Act
            var result = VacWebSiteTools.VacTools.GetTeachers(param);
            //Assert
            Assert.AreEqual(expectedFirstElm, result.First());
            Assert.AreEqual(expectedCount, result.Count);
        }

        [TestMethod()]
        public void Get_代入匯入時講師名稱_講師陣列有空值時回傳null()
        {
            //Arrange
            var param = "李xx,,黃娟娟";
            //Act
            var result = VacWebSiteTools.VacTools.GetTeachers(param);
            //Assert
            Assert.IsNull(result);
        }

    }
}