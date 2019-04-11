using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class VacTools
    {
        /// <summary>
        /// 配當表匯入時Split教師欄位逗號分隔
        /// </summary>
        /// <param name="teacherData"></param>
        /// <returns></returns>
        public static List<string> GetTeachers(string teacherData)
        {
            var result =  teacherData.Split(',').ToList();

            return result.Any(p => p == "") ? null : result;
        }

    }
}
