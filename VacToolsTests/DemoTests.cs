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
    public class DemoTests
    {
        [TestMethod()]
        public void Fun1Test()
        {
            //Arrange
            var demo = new Demo("xxx");
            var excepted = 7;
            //Act
            var result = demo.Fun1();
            //Assert
            Assert.AreEqual(excepted, result);
        }

        [TestMethod()]
        public void Fun2Test()
        {
            //Arrange
            var demo = new Demo();
            var excepted = 10;
            //Act
            var result = demo.Fun2();
            //Assert
            Assert.AreEqual(excepted, result);
        }

        [TestMethod()]
        public void Fun3Test＿應該回傳３()
        {
            //Arrange
            var demo = new Demo();
            var excepted = 3;
            //Act
            var result = demo.Fun3();
            //Assert
            Assert.AreEqual(excepted, result);

        }
    }
}