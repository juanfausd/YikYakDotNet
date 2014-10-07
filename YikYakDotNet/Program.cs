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
            api.Execute();
        }
    }
}
