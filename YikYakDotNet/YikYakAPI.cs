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
        private const string USER_AGENT = "android-async-http/1.4.4 (http://loopj.com/android-async-http)";

        private string GenerateUserId()
        {
            return new Guid().ToString();
        }

        public void Execute()
        {
            string key = "35FD04E8-B7B1-45C4-9886-94A75F4A2BB4";
            string salt = ConvertToUnixTimestamp(DateTime.Now).ToString();

            string url = GET_MESSAGES_URL;
            url = url.Replace("{latitude}", "40.013842");
            url = url.Replace("{longitude}", "-83.031085");
            url = url.Replace("{user-id}", Guid.NewGuid().ToString().Replace("-", ""));
            string encodeUrl = url;
            encodeUrl += salt;

            string hash = Encode(encodeUrl, key);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);

            url = "https://yikyakapp.com" + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            string content = Helpers.ReadWebResponse(response);
        }

        public static double ConvertToUnixTimestamp(DateTime date)
        {
            DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan diff = date.ToUniversalTime() - origin;
            return Math.Ceiling(diff.TotalSeconds);
        }

        public static string Encode(string input, string key)
        {
            var keyMaterial = System.Text.Encoding.UTF8.GetBytes(key);
            HMACSHA1 myhmacsha1 = new HMACSHA1(keyMaterial);
            byte[] byteArray = System.Text.Encoding.UTF8.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            byte[] res = myhmacsha1.ComputeHash(stream);
            var b64 = Convert.ToBase64String(res);
            return System.Uri.EscapeDataString(b64);
        }
    }
}
