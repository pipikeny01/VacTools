using System;
using System.Collections.Generic;
using VacWebSiteTools.ODT;

namespace VacWebSiteTools.Helper
{
    public interface ITableColmunsHelper
    {
        Tuple<List<string>, List<string>> GetTableColumnsForCommand(string tableName, List<string> removeColumns = null);
        List<TableColmuns> SelectableColumnsName(string tableName);
    }
}