using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class PayDanHelper : IPayDanHelper
    {
        IDateHelper _dateHelper;
        public PayDanHelper(IDateHelper dateHelper)
        {
            _dateHelper = dateHelper;
        }

        /// <summary>
        /// 取得配當表表頭周次及日期
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<PayDanWeeks> GetPayDanWeeks(DateTime startDate, DateTime endDate)
        {
            var titles = new List<PayDanWeeks>();
            var totalWeek = _dateHelper.GetTotalWeekCount(startDate, endDate);

            //開始的日期要先知道是星期幾,讓第一周的結束日期去扣掉
            var startDateWeek = _dateHelper.GetWeekOfDate(startDate);

            for (int i = 0; i < totalWeek; i++)
            {
                var sDate = startDate;

                var title = new PayDanWeeks
                {
                    StarDate = sDate,
                    EndDate = (i == 0) ? sDate.AddDays(6 - startDateWeek) : sDate.AddDays(6),
                    Week = (i + 1).ToString()
                };

                startDate = title.EndDate.AddDays(1);

                //最後一筆只要顯示結束條件的日期
                if (i + 1 == totalWeek)
                    title.EndDate = endDate;

                titles.Add(title);
            }

            return titles;
        }
    }
}