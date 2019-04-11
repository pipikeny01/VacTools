using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    /// <summary>
    /// 未重構前
    /// </summary>
    public class PayDanHelper_v1
    {
        /// <summary>
        /// 取得配當表表頭周次及日期
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public List<PayDanTitle> GetPayDanTitles(DateTime startDate, DateTime endDate)
        {
            var dateHelper = new DateHelper();
            var titles = new List<PayDanTitle>();
            var totalWeek = dateHelper.GetTotalWeekCount(startDate, endDate);

            //開始的日期要先知道是星期幾,讓第一周的結束日期去扣掉
            var startDateWeek = dateHelper.GetWeekOfDate(startDate);

            for (int i = 0; i < totalWeek; i++)
            {
                var sDate = startDate;
                //sDate = startDate.AddDays(i * 7);

                var title = new PayDanTitle
                {
                    StarDate = sDate,
                    EndDate = (i == 0) ? sDate.AddDays(6 - startDateWeek) : sDate.AddDays(6),
                    Week = (i + 1).ToString()
                };

                startDate = title.EndDate.AddDays(1);

                titles.Add(title);
            }

            return titles;
        }
    }

}