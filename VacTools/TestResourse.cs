using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class TestResourse
    {
        public string GetRes()
        {
            return MyResources.BaseResource.GetString("Address");
        }
    }
}
