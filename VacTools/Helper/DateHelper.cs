using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class DateHelper : IDateHelper
    {
        /// <summary>
        /// 取得輸入日期是該年第幾周
        /// </summary>
        /// <returns></returns>
        public int GetWeekOfYearFromDate(DateTime date)
        {
            GregorianCalendar gc = new GregorianCalendar();
            int weekOfYear = gc.GetWeekOfYear(date, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
            return weekOfYear;
        }

        /// <summary>
        /// 取得日期區間共有幾周
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public int GetTotalWeekCount(DateTime startDate, DateTime endDate)
        {
            var startWeek = GetWeekOfYearFromDate(startDate);
            var endWeek = GetWeekOfYearFromDate(endDate);
            return endWeek - startWeek + 1;
        }

        /// <summary>
        /// 獲取當前時間是本月的第幾周
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
        public int GetWeekNumInMonth(DateTime daytime)
        {
            int dayInMonth = daytime.Day;
            //本月第一天
            DateTime firstDay = daytime.AddDays(1 - daytime.Day);
            //本月第一天是周幾
            int weekday = (int)firstDay.DayOfWeek == 0 ? 7 : (int)firstDay.DayOfWeek;
            //本月第一周有幾天
            int firstWeekEndDay = 7 - (weekday - 1);
            //當前日期和第一周之差
            int diffday = dayInMonth - firstWeekEndDay;
            diffday = diffday > 0 ? diffday : 1;
            //當前是第幾周,如果整除7就減一天
            int WeekNumInMonth = ((diffday % 7) == 0
             ? (diffday / 7 - 1)
             : (diffday / 7)) + 1 + (dayInMonth > firstWeekEndDay ? 1 : 0);
            return WeekNumInMonth;
        }

        /// <summary>
        /// 取得日期是星期幾
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public int GetWeekOfDate(DateTime date)
        {
            return Convert.ToInt32(date.DayOfWeek.ToString("d"));
        }

        /// <summary>
        /// 日期的次日 , 但是會跳過六,日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetNextDateSkipHoliday(DateTime date)
        {
            date = date.AddDays(1);
            var week = GetWeekOfDate(date);
            if(week == 6 || week == 0)
            {
               return GetNextDateSkipHoliday(date.AddDays(1));
            }

            return date;
        }


        /// <summary>
        /// 日期的次日 , 但是只會是六,日
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        public DateTime GetNextDateOnlyHoliday(DateTime date)
        {
            date = date.AddDays(1);
            var week = GetWeekOfDate(date);
            if (week != 6 && week != 0)
            {
                return GetNextDateOnlyHoliday(date.AddDays(1));
            }

            return date;
        }

        /// <summary>
        /// 西元、民國互轉
        /// </summary>
        /// <param name="date"></param>
        /// <param name="type">Defalut:false, true民國轉西元, false西元轉民國 </param>
        /// <returns></returns>
        public static string DateTimeCovert(string date, bool type = false)
        {
            if (string.IsNullOrEmpty(date)) return "";

            if (type)
            {
                //民國轉西元
                string[] tDate = date.Split(new Char[] { '/' });
                //kenny 左邊補0 ,比字串會有問題
                var tDate2 = tDate[1].ToString().PadLeft(2, '0');
                var tDate3 = tDate[2].ToString().PadLeft(2, '0');
                //date = date.Replace(tDate[0], (int.Parse(tDate[0]) + 1911).ToString());
                date = string.Format("{0}/{1}/{2}", (int.Parse(tDate[0]) + 1911).ToString(), tDate2, tDate3);

                return date;
            }
            else
            {
                //西元轉民國
                DateTime d = DateTime.Parse(date);
                DateTime minDate = DateTime.Parse("1912/01/01");
                TaiwanCalendar tCalendar = new TaiwanCalendar();
                if (d >= minDate)
                {
                    return string.Format("{0}/{1}/{2}", tCalendar.GetYear(d), d.Month.ToString().PadLeft(2, '0'), d.Day.ToString().PadLeft(2, '0'));
                }
                else
                {
                    return string.Empty;
                }
            }
        }
        public static string DateTimeCoverttime(string date, bool type = false)
        {
            if (string.IsNullOrEmpty(date)) return "";

            if (type)
            {
                //民國轉西元
                string[] tDate = date.Split(new Char[] { '/' });
                //kenny 左邊補0 ,比字串會有問題
                var tDate2 = tDate[1].ToString().PadLeft(2, '0');
                var tDate3 = tDate[2].ToString().PadLeft(2, '0');
                //date = date.Replace(tDate[0], (int.Parse(tDate[0]) + 1911).ToString());
                date = string.Format("{0}/{1}/{2}", (int.Parse(tDate[0]) + 1911).ToString(), tDate2, tDate3);

                return date;
            }
            else
            {
                //西元轉民國
                DateTime d = DateTime.Parse(date);
                DateTime minDate = DateTime.Parse("1912/01/01");
                TaiwanCalendar tCalendar = new TaiwanCalendar();
                if (d >= minDate)
                {
                    return string.Format("{0}/{1}/{2} {3}:{4}", tCalendar.GetYear(d), d.Month.ToString().PadLeft(2, '0'), d.Day.ToString().PadLeft(2, '0'), d.Hour.ToString().PadLeft(2, '0'), d.Minute.ToString().PadLeft(2, '0'));
                }
                else
                {
                    return string.Empty;
                }
            }
        }

    }
}