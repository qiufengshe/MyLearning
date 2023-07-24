using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CharpLearning.Syntax
{
    public class InterpolatedStringTest
    {
        public string Test1()
        {
            int a = 20;
            string result = $"a={a}";
            return result;
        }
    }
}
