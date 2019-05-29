using System;
using System.Collections.Generic;
using System.Data;
using VacWebSiteTools.Service;

namespace VacWebSiteTools.Helper
{
    /// <summary>
    /// 教室排課資格條件定義
    /// </summary>
    public interface ITeacherQualifi
    {
        /// <summary>
        /// 教師資格條件列表
        /// </summary>
        /// <param name="dv"></param>
        /// <returns></returns>
        Dictionary<Func<bool>, Func<CheckQualifiResult>> GetQualifiList(DataView dv);
    }
}