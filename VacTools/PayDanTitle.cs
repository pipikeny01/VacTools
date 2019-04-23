using System;

namespace VacWebSiteTools
{
    /// <summary>
    /// 配當表的表頭資料結構
    /// </summary>
    public class PayDanWeeks
    {
        public string Week { set; get; }

        public DateTime StarDate { set; get; }

        public DateTime EndDate { set; get; }
    }
}