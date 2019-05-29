using aiet.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VacWebSiteTools.Factory;

namespace VacWebSiteTools.Service
{
    public abstract class BaseService
    {
        protected tools tool;
        protected DateHelper _dateHelper;

        public BaseService()
        {
            _dateHelper = HelperFactory.GetDateHelper();
            tool = new tools();
        }
    }
}
