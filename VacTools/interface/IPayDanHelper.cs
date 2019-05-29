using System;
using System.Collections.Generic;

namespace VacWebSiteTools
{
    public interface IPayDanHelper
    {
        List<PayDanWeeks> GetPayDanWeeks(DateTime startDate, DateTime endDate);
    }
}