using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet
{
    public class YikYakAPI
    {
        private const string GET_MESSAGES_URL = "/api/getMessages?lat={latitude}&long={longitude}&userID={user-id}";
        private const string REGISTER_USER_URL = "/api/registerUser?lat={latitude}&long={longitude}&userID={user-id}";
        private const string USER_AGENT = "Mozilla/5.1 (Linux; Android 4.0.4; Galaxy Nexus Build/IMM76B) AppleWebKit/535.19 (KHTML, like Gecko) Chrome/18.0.1025.133 Mobile Safari/535.19";
        private const string DEVICE_KEY = "35FD04E8-B7B1-45C4-9886-94A75F4A2BB4";

        public void Execute()
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            string userId = Helpers.CalculateMD5Hash(Guid.NewGuid().ToString());

            RegisterUser(userId, DEVICE_KEY, salt);

            string result = GetMessages(userId, DEVICE_KEY, salt);
        }

        public void RegisterUser(string userId, string key, string salt)
        {
            string regUrl = REGISTER_USER_URL;
            regUrl = regUrl.Replace("{latitude}", "38.832194");
            regUrl = regUrl.Replace("{longitude}", "-77.308037");
            regUrl = regUrl.Replace("{user-id}", userId);
            string encodeRegUrl = regUrl;
            encodeRegUrl += salt;

            string regHash = Helpers.Encode(encodeRegUrl, key);
            regUrl = regUrl + "&salt={salt}".Replace("{salt}", salt);
            regUrl = regUrl + "&hash={hash}".Replace("{hash}", regHash);
            regUrl = "https://yikyakapp.com" + regUrl;

            HttpWebRequest regRequest = WebRequest.Create(regUrl) as HttpWebRequest;
            regRequest.Method = "GET";
            regRequest.UserAgent = USER_AGENT;
            HttpWebResponse regResponse = (HttpWebResponse)regRequest.GetResponse();
        }

        public string GetMessages(string userId, string key, string salt)
        {
            string url = GET_MESSAGES_URL;
            url = url.Replace("{latitude}", "38.832194");
            url = url.Replace("{longitude}", "-77.308037");
            url = url.Replace("{user-id}", userId);
            string encodeUrl = url;
            encodeUrl += salt;

            string hash = Helpers.Encode(encodeUrl, key);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);
            url = "https://yikyakapp.com" + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return Helpers.ReadWebResponse(response);
        }
    }
}
