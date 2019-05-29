using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.UI.WebControls;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class clsParametersTests
    {
        [TestMethod()]
        public void CombinWhereInTest組出WhereIn的條件和參數()
        {
           var clsParam = new clsParameters();
           var expect1 = "@test1,@test2,@test3";
           var expect2 = new List<ListItem>
           {
               new ListItem("test1","v1"),
               new ListItem("test2","v2"),
               new ListItem("test3","v3"),
           };

           var result = clsParam.CombinWhereIn("test", "v1", "v2", "v3");
           Assert.AreEqual(expect1,result.Item1);

           foreach (var item in result.Item2)
               Assert.AreEqual(expect2.FirstOrDefault(p=>p.Text == item.Text).Value, item.Value);

        }
    }
}