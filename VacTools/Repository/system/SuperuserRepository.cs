using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools.Repository
{
    public class SuperuserRepository
    {
        /// <summary>
        /// 取得系統內定superuser帳號
        /// </summary>
        /// <returns></returns>
        public static List<string> SelectSuperusers()
        {
            return new List<string> {"admin"};
        }

        /// <summary>
        /// 輸入帳號是否是superuser帳號
        /// </summary>
        /// <param name="account"></param>
        /// <returns></returns>
        public static bool ContainsSuperuser(string account)
        {
            var accounts = SelectSuperusers();
            return accounts.Contains(account);
        }
    }
}
