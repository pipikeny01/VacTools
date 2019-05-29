using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace VacWebSiteTools
{
    public class MyApp
    {
        public static bool ForTesting { set; get; }
        /// <summary>
        /// 後台使用者
        /// </summary>
        public static UserInfo SysUser
        {
            get
            {
                if (ForTesting)
                    return GetTestingUserInfo();

                var hx = HttpContext.Current;
                if (hx.Session != null && hx.Session["userinfo"] != null)
                {
                    try
                    {
                        var u = hx.Session["userinfo"] as UserInfo;

                        if (u == null)
                            hx.Response.Redirect("~/assistportal/assist.aspx", true);

                        u.MemberNo = u.Accnt;

                        return u;
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }

                return null;
            }
        }

        /// <summary>
        /// 前台使用者
        /// </summary>
        public static UserInfo Member
        {//
            get
            {
                if (ForTesting)
                    return GetTestingUserInfo();

                var hx = HttpContext.Current;
                if (hx.Session != null && hx.Session["memberInfo"] != null)
                {
                    return hx.Session["memberInfo"] as UserInfo;
                }

                return null;
            }
        }

        public static UserInfo GetTestingUserInfo()
        {
            return new UserInfo { Accnt = "admin", isSysAdmin = true };

        }
    }

    public interface IMyApp
    {
        
    }
}
