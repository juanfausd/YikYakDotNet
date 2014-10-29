using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YikYakDotNet.Entities;

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
        List<Yak> GetYaks(double latitude, double longitude);

        /// <summary>
        /// Gets YikYak posts for a certain location. It must be considered that the API uses a
        /// radius of 5 miles, and it can't be changed when invoking to getMessages method.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        string GetMessages(double latitude, double longitude);

        /// <summary>
        /// Gets YikYak posts for a certain location. It's a overload of GetMessages(double latitude, double longitude).
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string GetMessages(double latitude, double longitude, string userId, string salt);

        /// <summary>
        /// Registers a UserID in a specific location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="salt"></param>
        void RegisterUser(double latitude, double longitude, string userId, string salt);
    }
}
