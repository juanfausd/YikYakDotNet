using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YikYakDotNet.Entities;

namespace YikYakDotNet
{
    /// <summary>
    /// Yik Yak is a geographic based chatting application that allows users to anonymously post anything 
    /// for their area and read what other users do. Actually, YikYak allows message to be 200 characters 
    /// longer and the radius used in the geographic search is 5 miles.
    /// The purpose of this API is to provide a tool to other developers that are interested on consuming 
    /// YikYak data. Specially for those who use .NET.
    /// </summary>
    public class YikYakAPI : IYikYakAPI
    {
        private const string GET_MESSAGES_URL = "/api/getMessages?lat={latitude}&long={longitude}&userID={user-id}&version={version}";
        private const string REGISTER_USER_URL = "/api/registerUser?lat={latitude}&long={longitude}&userID={user-id}&version={version}";
        private const string BASE_URL = "https://us-east-api.yikyakapi.net";
        private const string USER_AGENT = "Yik Yak/2.1.0.23 CFNetwork/711.1.12 Darwin/14.0.0";
        private const string DEVICE_KEY = "F7CAFA2F-FE67-4E03-A090-AC7FFF010729";
        private const string VERSION = "2.1.003";

        public List<Yak> GetYaks(double latitude, double longitude)
        {
            string res = GetMessages(latitude, longitude);

            var deserializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
            };

            JToken token = JsonConvert.DeserializeObject<JObject>(res, deserializerSettings);

            List<Yak> result = new List<Yak>();

            foreach(JToken messageToken in token["messages"])
            {
                Yak yak = new Yak();
                yak.MessageID = messageToken["messageID"].ToString();
                yak.Message = messageToken["message"].ToString();
                yak.Latitude = double.Parse(messageToken["latitude"].ToString(), CultureInfo.InvariantCulture);
                yak.Longitude = double.Parse(messageToken["longitude"].ToString(), CultureInfo.InvariantCulture);
                yak.Time = DateTime.ParseExact((string)messageToken["time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                result.Add(yak);
            }

            return result;
        }

        public List<Yak> GetYaks(double latitude, double longitude, string userId, string salt)
        {
            string res = GetMessages(latitude, longitude, userId, salt);

            var deserializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
            };

            JToken token = JsonConvert.DeserializeObject<JObject>(res, deserializerSettings);

            List<Yak> result = new List<Yak>();

            foreach (JToken messageToken in token["messages"])
            {
                Yak yak = new Yak();
                yak.MessageID = messageToken["messageID"].ToString();
                yak.Message = messageToken["message"].ToString();
                yak.Latitude = double.Parse(messageToken["latitude"].ToString(), CultureInfo.InvariantCulture);
                yak.Longitude = double.Parse(messageToken["longitude"].ToString(), CultureInfo.InvariantCulture);
                yak.Time = DateTime.ParseExact((string)messageToken["time"], "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
                result.Add(yak);
            }

            return result;
        }

        public string GetMessages(double latitude, double longitude)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            //string userId = Helpers.CalculateMD5Hash(Guid.NewGuid().ToString());
            string userId = Guid.NewGuid().ToString().ToUpper();

            RegisterUser(latitude, longitude, userId, salt);

            return GetMessages(latitude, longitude, userId, salt);
        }

        public string GetMessages(double latitude, double longitude, string userId, string salt)
        {
            string url = GET_MESSAGES_URL;
            url = url.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-id}", userId);
            url = url.Replace("{version}", VERSION);
            string encodeUrl = url;
            encodeUrl += salt;

            string hash = Helpers.Encode(encodeUrl, DEVICE_KEY);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);
            url = BASE_URL + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return Helpers.ReadWebResponse(response);
        }

        public void RegisterUser(double latitude, double longitude, string userId, string salt)
        {
            string regUrl = REGISTER_USER_URL;
            regUrl = regUrl.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            regUrl = regUrl.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            regUrl = regUrl.Replace("{user-id}", userId);
            regUrl = regUrl.Replace("{version}", VERSION);
            string encodeRegUrl = regUrl;
            encodeRegUrl += salt;

            string regHash = Helpers.Encode(encodeRegUrl, DEVICE_KEY);
            regUrl = regUrl + "&salt={salt}".Replace("{salt}", salt);
            regUrl = regUrl + "&hash={hash}".Replace("{hash}", regHash);
            regUrl = BASE_URL + regUrl;

            HttpWebRequest regRequest = WebRequest.Create(regUrl) as HttpWebRequest;
            regRequest.Method = "GET";
            regRequest.UserAgent = USER_AGENT;
            HttpWebResponse regResponse = (HttpWebResponse)regRequest.GetResponse();
        }
    }
}
