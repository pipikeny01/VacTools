using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacWebSiteTools.Helper;

namespace VacWebSiteTools.Service
{
    public enum CheckQualifiResult
    {
        InOverHour,
        OutOverHour,
        OverYearsOld,
        NoProblem
    }

    public class EditLessonRndService : BaseService
    {
        private IclsEducationLecturer _clsEducationLecturer;
        private ITeacherQualifi _teacherQualifi;

        public EditLessonRndService()
        {
            _clsEducationLecturer = RepositoryFactory.GetClsEducationLecturer();
            _teacherQualifi = RepositoryFactory.GetTeacherQualifi();
        }

        public EditLessonRndService(ITeacherQualifi teacherQualifi, IclsEducationLecturer clsEducationLecturer)
        {
            _teacherQualifi = teacherQualifi;
            _clsEducationLecturer = clsEducationLecturer;
        }

        /// <summary>
        /// 檢查教師排課資格 年紀及時數檢查
        /// </summary>
        /// <param name="teacher"></param>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <returns></returns>
        public virtual CheckQualifiResult CheckQualifi(string teacher, string d1, string d2)
        {
            var dv = _clsEducationLecturer.SelectTeacherHourCount(teacher, d1, d2);
            var qualifiList = _teacherQualifi.GetQualifiList(dv);
            foreach (var kv in qualifiList)
            {
                if (kv.Key())
                    return kv.Value();
            }

            //留下舊code當作範例
            //if (dv.Count > 0)
            //{
            //    if (Convert.ToInt16(dv[0]["yearsold"]) > _clsParameters.TeacherQualityualifi.Age_limit)
            //    {
            //        return CheckQualityResult.OverYearsOld;
            //    }

            //    if (dv[0]["TeacherKind"].ToString() == TeacherKindDefine.內聘專任訓練師
            //        && Convert.ToInt16(dv[0]["HourCount"]) > _clsParameters.TeacherQualityualifi.Iteacher_hour_limit)
            //    {
            //        return CheckQualityResult.InOverHour;
            //    }
            //    else
            //    {
            //        if  (Convert.ToInt16(dv[0]["HourCount"]) > _clsParameters.TeacherQualityualifi.Oteacher_hour_limit)
            //        {
            //            return CheckQualityResult.OutOverHour;
            //        }
            //    }
            //}

            return CheckQualifiResult.NoProblem;
        }
    }
}