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

        public  Age CalculateAge(DateTime birthDate, DateTime endDate)
        {
            if (birthDate.Date > endDate.Date)
                throw new ArgumentException("birthDate cannot be higher then endDate", "birthDate");

            int years = endDate.Year - birthDate.Year;
            int months = 0;
            int days = 0;

            // Check if the last year, was a full year.
            if (endDate < birthDate.AddYears(years) && years != 0)
                years--;

            // Calculate the number of months.
            birthDate = birthDate.AddYears(years);

            if (birthDate.Year == endDate.Year)
                months = endDate.Month - birthDate.Month;
            else
                months = (12 - birthDate.Month) + endDate.Month;

            // Check if last month was a complete month.
            if (endDate < birthDate.AddMonths(months) && months != 0)
                months--;

            // Calculate the number of days.
            birthDate = birthDate.AddMonths(months);

            days = (endDate - birthDate).Days;

            return new Age
            {
                Years = years,
                Months = months,
                Days = days
            };
        }
    }
}