using System;
using System.Collections.Generic;
using System.Data;
using VacWebSiteTools.Service;

namespace VacWebSiteTools.Helper
{
    /// <summary>
    /// 教室排課資格條件定義
    /// </summary>
    public class TeacherQualifi : ITeacherQualifi
    {
        private IclsParameters _clsParameters;
        public TeacherQualifi()
        {
            _clsParameters = RepositoryFactory.GetClsParameters();
        }

        public TeacherQualifi(IclsParameters clsParameters)
        {
            _clsParameters = clsParameters;
        }

        /// <summary>
        /// 教師資格條件列表
        /// </summary>
        /// <param name="dv"></param>
        /// <returns></returns>
        public virtual Dictionary<Func<bool>, Func<CheckQualifiResult>> GetQualifiList(DataView dv)
        {
            return new Dictionary<Func<bool>, Func<CheckQualifiResult>>
            {
                {
                    () => Convert.ToInt16(dv[0]["yearsold"]) > _clsParameters.TeacherQualityualifi.Age_limit,
                    ()=>  CheckQualifiResult.OverYearsOld
                },
                {
                    () => dv[0]["TeacherKind"].ToString() == TeacherKindDefine.內聘專任訓練師
                          && Convert.ToInt16(dv[0]["HourCount"]) > _clsParameters.TeacherQualityualifi.Iteacher_hour_limit,
                    ()=>  CheckQualifiResult.InOverHour
                },
                {
                    () => dv[0]["TeacherKind"].ToString() == TeacherKindDefine.外聘導師
                          && Convert.ToInt16(dv[0]["HourCount"]) > _clsParameters.TeacherQualityualifi.Oteacher_hour_limit,
                    ()=>  CheckQualifiResult.OutOverHour
                },
            };
        }

    }
}