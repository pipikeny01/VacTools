using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class RepositoryFactory
    {
        public static clsEducation_level GetClsEducation_level()
        {
            return new clsEducation_level();
        }
    }
}
