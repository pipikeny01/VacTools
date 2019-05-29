using VacWebSiteTools.Service;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using VacWebSiteTools.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacToolsTests.Stub;
using System.Data;
using VacWebSiteTools.ODT;
using NSubstitute;
using Aiet_DB;

namespace VacWebSiteTools.Tests
{
    [TestClass()]
    public class TableColmunsServiceTests
    {
        private ITableColmunsHelper _tbService;
        baseDB _mock = Substitute.For<baseDB>();

        [TestInitialize]
        public void Initial()
        {
            _tbService = new TableColmunsHelper(_mock);
        }

        [TestMethod()]
        public void SelectableColumnsNameTest取得TableColmuns()
        {
            var expectList = new List<TableColmuns>();
            expectList.Add(new TableColmuns { ColumnName = "aID", ColumnType = "int(11)", IsNullable = false });
            expectList.Add(new TableColmuns { ColumnName = "APVMailID", ColumnType = "varchar(10)", IsNullable = true });
            expectList.Add(new TableColmuns { ColumnName = "Test1", ColumnType = "varchar(10)", IsNullable = true });

            _mock.SelectTableColumnsInfo(Arg.Any<string>()).Returns(SelectTableColumnsInfo());

            var result = _tbService.SelectableColumnsName("lesson");

            Assert.AreEqual(expectList.Count, result.Count);

            for (int i = 0; i < expectList.Count; i++)
                Assert.AreEqual(expectList[i].ColumnName, result[i].ColumnName);
        }

        [TestMethod()]
        public void GetTableColumnsForCommandTest回傳Table欄位字串()
        {
            var expectItem1 = new List<string> { "aID,APVMailID,Test1" };
            var expectItem2 = new List<string> { "@aID,@APVMailID,@Test1" };

            _mock.SelectTableColumnsInfo(Arg.Any<string>()).Returns(SelectTableColumnsInfo());

            var result = _tbService.GetTableColumnsForCommand("");

            Assert.AreEqual(string.Join(",", expectItem1), string.Join(",", result.Item1));
            Assert.AreEqual(string.Join(",", expectItem2), string.Join(",", result.Item2));
        }

        private DataView SelectTableColumnsInfo()
        {
            var dt = new DataTable();
            dt.Columns.Add("COLUMN_NAME");
            dt.Columns.Add("COLUMN_TYPE");
            dt.Columns.Add("IS_NULLABLE");

            var newRow = dt.NewRow();
            newRow["COLUMN_NAME"] = "aID";
            newRow["COLUMN_TYPE"] = "int(11)";
            newRow["IS_NULLABLE"] = "NO";

            dt.Rows.Add(newRow);

            newRow = dt.NewRow();
            newRow["COLUMN_NAME"] = "APVMailID";
            newRow["COLUMN_TYPE"] = "varchar(10)";
            newRow["IS_NULLABLE"] = "YES";

            dt.Rows.Add(newRow);

            newRow = dt.NewRow();
            newRow["COLUMN_NAME"] = "Test1";
            newRow["COLUMN_TYPE"] = "varchar(10)";
            newRow["IS_NULLABLE"] = "YES";

            dt.Rows.Add(newRow);

            return dt.DefaultView;


        }

        [TestMethod()]
        public void GetTableColumnsForCommandTest不包含APVMailID和aID只有Test1()
        {
            var expectItem1 = new List<string> { "Test1"};

            _mock.SelectTableColumnsInfo(Arg.Any<string>()).Returns(SelectTableColumnsInfo());

            var removeColmns = new List<string> { "APVMailID", "aID" };
            var result = _tbService.GetTableColumnsForCommand("", removeColmns);

            Assert.AreEqual(string.Join(",", expectItem1), string.Join(",", result.Item1));
        }
    }
}