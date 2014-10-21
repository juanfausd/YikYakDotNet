using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet
{
    public interface IYikYakAPI
    {
        /// <summary>
        /// Gets YikYak posts for a certain location. It must be considered that the API uses a
        /// radius of 5 miles, and it can't be changed when invoking to getMessages method.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        string GetMessages(double latitude, double longitude);
    }
}
