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
        /// <param name="userId"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        List<Yak> GetYaks(double latitude, double longitude, string userId, string salt);

        /// <summary>
        /// Get featured locations.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        List<Location> GetFeaturedLocations(double latitude, double longitude);

        /// <summary>
        /// Get featured locations.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        List<Location> GetFeaturedLocations(double latitude, double longitude, string userId, string salt);

        /// <summary>
        /// Get other locations.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        List<Location> GetOtherLocations(double latitude, double longitude);

        /// <summary>
        /// Get other locations.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        List<Location> GetOtherLocations(double latitude, double longitude, string userId, string salt);

        /// <summary>
        /// Get locations of a certain type.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<Location> GetLocations(double latitude, double longitude, string type);

        /// <summary>
        /// Get locations of a certain type.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="salt"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        List<Location> GetLocations(double latitude, double longitude, string userId, string salt, string type);

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

        /// <summary>
        /// Peek messages from a certain location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="peekId"></param>
        /// <returns></returns>
        string PeekMessages(double latitude, double longitude, string peekId);

        /// <summary>
        /// Peek messages from a certain location.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="peekId"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string PeekMessages(double latitude, double longitude, string userId, string peekId, string salt);

        /// <summary>
        /// Peeks anywhere.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userLatitude"></param>
        /// <param name="userLongitude"></param>
        /// <returns></returns>
        string Yaks(double latitude, double longitude, double userLatitude, double userLongitude);

        /// <summary>
        /// Peeks anywhere.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <param name="userLatitude"></param>
        /// <param name="userLongitude"></param>
        /// <param name="salt"></param>
        /// <returns></returns>
        string Yaks(double latitude, double longitude, string userId, double userLatitude, double userLongitude, string salt);
    }
}
