using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet
{
    class Program
    {
        static void Main(string[] args)
        {
            YikYakAPI api = new YikYakAPI();
            var res = api.GetMessages(38.832194, -77.308037);
        }
    }
}
