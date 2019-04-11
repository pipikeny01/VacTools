using System;

namespace VacWebSiteTools
{
    public interface IDateHelper
    {
        /// <summary>
        /// 取得輸入日期是該年第幾周
        /// </summary>
        /// <returns></returns>
        int GetTotalWeekCount(DateTime startDate, DateTime endDate);

        /// <summary>
        /// 取得日期區間共有幾周
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        int GetWeekNumInMonth(DateTime daytime);
        /// <summary>
        /// 獲取當前時間是本月的第幾周
        /// </summary>
        /// <param name="daytime"></param>
        /// <returns></returns>
        int GetWeekOfDate(DateTime date);
        /// <summary>
        /// 取得日期是星期幾
        /// </summary>
        /// <param name="date"></param>
        /// <returns></returns>
        int GetWeekOfYearFromDate(DateTime date);
    }
}