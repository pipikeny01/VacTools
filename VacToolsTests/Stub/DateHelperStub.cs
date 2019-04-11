using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacWebSiteTools;

namespace VacToolsTests.Stub
{
    public class DateHelperStub : IDateHelper
    {
        public int GetTotalWeekCount(DateTime startDate, DateTime endDate)
        {
            return 24;
        }

        public int GetWeekNumInMonth(DateTime daytime)
        {
            throw new NotImplementedException();
        }

        public int GetWeekOfDate(DateTime date)
        {
            return 3;
        }

        public int GetWeekOfYearFromDate(DateTime date)
        {
            throw new NotImplementedException();
        }
    }
}
