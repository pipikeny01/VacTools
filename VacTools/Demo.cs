using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VacWebSiteTools
{
    public class Demo
    {
        public Demo()
        {

        }

        public Demo(string test):this()
        {

        }

        private int _baseNumber = 2;
        public int Fun1()
        {
            return _baseNumber + 5;
        }

        public object Fun2()
        {
            return _baseNumber + 8;
        }

        public object Fun3()
        {
            return _baseNumber + 1;
        }

    }
}
