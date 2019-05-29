using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools.Factory
{
    public class HelperFactory
    {
        public static DateHelper GetDateHelper()
        {
            return new DateHelper();
        }
    }
}
