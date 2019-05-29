using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class clsEducationLecturerTests
    {
        [TestMethod()]
        public void ValidateIDNOPassTest教師身分是無沒填身分證或生日會return_true()
        {
            var cls = new clsEducationLecturer();
            var expect = true;
            var result = cls.ValidateIDNOPass("0", "", "");
            Assert.AreEqual(expect,result);
        }

        [TestMethod()]
        public void ValidateIDNOPassTest教師身分不是無沒填身分證或生日會return_false()
        {
            var cls = new clsEducationLecturer();
            var expect = false;
            var result = cls.ValidateIDNOPass("A", "12", "");
            Assert.AreEqual(expect, result);
        }

    }
}