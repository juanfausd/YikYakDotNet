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
using System.Threading;
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
        private const string PEEK_MESSAGES_URL = "/api/getPeekMessages?lat={latitude}&long={longitude}&userID={user-id}&peekID={peek-id}&version={version}";
        private const string YAKS_URL = "/api/getMessages?lat={latitude}&long={longitude}&userID={user-id}&userLat={user-latitude}&userLong={user-longitude}&version={version}";
        //private const string BASE_URL = "https://us-east-api.yikyakapi.net";
        private const string BASE_URL = "https://us-central-api.yikyakapi.net";
        private const string USER_AGENT = "Dalvik/1.6.0 (Linux; U; Android 4.4.4; XT1080)" + " " + VERSION;
        private const string DEVICE_KEY = "";   // IMPORTANT: Request the new API KEY
        private const string VERSION = "2.6.1";

        public YikYakAPI(string deviceKey = DEVICE_KEY, string proxyAddress = null)
        {
            this.m_deviceKey = deviceKey;
            this.m_proxy_address = proxyAddress;

            if (!HasDeviceKey)
                throw new ArgumentNullException("Please specify a valid device key.");            
        }

        #region Private Members
        /// <summary>
        /// The device key to use when connecting to Yik Yak
        /// </summary>
        string m_deviceKey;
        /// <summary>
        /// Optional setting that specifies a proxy server to use for all requests
        /// </summary>
        string m_proxy_address;
        #endregion

        private bool HasDeviceKey
        {
            get { return string.IsNullOrEmpty(m_deviceKey) == false; }
        }

        #region User Management
        /// <summary>
        /// Returns a new unique UserId.
        /// 
        /// NOTE: 
        /// This userid must be Registered before it can be used
        /// with the YikYak API.
        /// </summary>
        /// <returns></returns>
        public string GenerateUserId()
        {
            return Guid.NewGuid().ToString().ToUpper();
        }
        public void RegisterUser(double latitude, double longitude, string userId)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            RegisterUser(latitude, longitude, userId, salt);
        }
        public void RegisterUser(double latitude, double longitude, string userId, string salt)
        {
            string regUrl = REGISTER_USER_URL;
            regUrl = regUrl.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            regUrl = regUrl.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            regUrl = regUrl.Replace("{user-id}", userId);
            regUrl = regUrl.Replace("{version}", VERSION);
            //regUrl = regUrl + "&bc=0";
            string encodeRegUrl = regUrl;
            encodeRegUrl += salt;

            string regHash = Helpers.Encode(encodeRegUrl, m_deviceKey);
            regUrl = regUrl + "&salt={salt}".Replace("{salt}", salt);
            regUrl = regUrl + "&hash={hash}".Replace("{hash}", regHash);
            regUrl = BASE_URL + regUrl;

            HttpWebRequest regRequest = WebRequest.Create(regUrl) as HttpWebRequest;
            regRequest.Method = "GET";
            regRequest.UserAgent = USER_AGENT;
            HttpWebResponse regResponse = (HttpWebResponse)regRequest.GetResponse();

            var content = Helpers.ReadWebResponse(regResponse);
            
        }

        public Task RegisterUserAsync(double latitude, double longitude, string userId)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            return RegisterUserAsync(latitude, longitude, userId, salt);
        }
        public Task RegisterUserAsync(double latitude, double longitude, string userId, string salt)
        {
            string regUrl = REGISTER_USER_URL;
            regUrl = regUrl.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            regUrl = regUrl.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            regUrl = regUrl.Replace("{user-id}", userId);
            regUrl = regUrl.Replace("{version}", VERSION);
            //regUrl = regUrl + "&bc=0";
            string encodeRegUrl = regUrl;
            encodeRegUrl += salt;

            string regHash = Helpers.Encode(encodeRegUrl, m_deviceKey);
            regUrl = regUrl + "&salt={salt}".Replace("{salt}", salt);
            regUrl = regUrl + "&hash={hash}".Replace("{hash}", regHash);
            regUrl = BASE_URL + regUrl;

            HttpWebRequest request = WebRequest.Create(regUrl) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;

            if (string.IsNullOrEmpty(m_proxy_address) == false)
                request.Proxy = new WebProxy(m_proxy_address);

            //var response = await request.GetResponseAsync();
            //var content = await Helpers.ReadWebResponseAsync(response);
            return request.GetResponseAsync();
        }
        #endregion

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

        public List<Location> GetFeaturedLocations(double latitude, double longitude)
        {
            return GetLocations(latitude, longitude, "featuredLocations");
        }

        public List<Location> GetFeaturedLocations(double latitude, double longitude, string userId, string salt)
        {
            return GetLocations(latitude, longitude, userId, salt, "featuredLocations");
        }

        public List<Location> GetOtherLocations(double latitude, double longitude)
        {
            return GetLocations(latitude, longitude, "otherLocations");
        }

        public List<Location> GetOtherLocations(double latitude, double longitude, string userId, string salt)
        {
            return GetLocations(latitude, longitude, userId, salt, "otherLocations");
        }

        public List<Location> GetLocations(double latitude, double longitude, string type)
        {
            string res = GetMessages(latitude, longitude);

            var deserializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
            };

            JToken token = JsonConvert.DeserializeObject<JObject>(res, deserializerSettings);

            List<Location> result = new List<Location>();

            foreach (JToken locationToken in token[type])
            {
                Location location = new Location();
                location.PeekID = locationToken["peekID"].ToString();
                location.Name = locationToken["location"].ToString();
                location.Inactive = locationToken["inactive"].ToString() == "1";
                location.CanSubmit = locationToken["canSubmit"].ToString() == "1";
                location.CanVote = locationToken["canVote"].ToString() == "1";
                location.CanReply = locationToken["canReply"].ToString() == "1";
                location.CanReport = locationToken["canReport"].ToString() == "1";
                location.Latitude = double.Parse(locationToken["latitude"].ToString(), CultureInfo.InvariantCulture);
                location.Longitude = double.Parse(locationToken["longitude"].ToString(), CultureInfo.InvariantCulture);
                location.Delta = double.Parse(locationToken["delta"].ToString(), CultureInfo.InvariantCulture);
                result.Add(location);
            }

            return result;
        }

        public List<Location> GetLocations(double latitude, double longitude, string userId, string salt, string type)
        {
            string res = GetMessages(latitude, longitude, userId, salt);

            var deserializerSettings = new JsonSerializerSettings()
            {
                DateFormatHandling = DateFormatHandling.IsoDateFormat,
                DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
            };

            JToken token = JsonConvert.DeserializeObject<JObject>(res, deserializerSettings);

            List<Location> result = new List<Location>();

            foreach (JToken locationToken in token[type])
            {
                Location location = new Location();
                location.PeekID = locationToken["peekID"].ToString();
                location.Name = locationToken["location"].ToString();
                location.Inactive = locationToken["inactive"].ToString() == "1";
                location.CanSubmit = locationToken["canSubmit"].ToString() == "1";
                location.CanVote = locationToken["canVote"].ToString() == "1";
                location.CanReply = locationToken["canReply"].ToString() == "1";
                location.CanReport = locationToken["canReport"].ToString() == "1";
                location.Latitude = double.Parse(locationToken["latitude"].ToString(), CultureInfo.InvariantCulture);
                location.Longitude = double.Parse(locationToken["longitude"].ToString(), CultureInfo.InvariantCulture);
                location.Delta = double.Parse(locationToken["delta"].ToString(), CultureInfo.InvariantCulture);
                result.Add(location);
            }

            return result;
        }

        /// <summary>
        /// Request messages from the Yik Yak API using a generated userid
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <returns></returns>
        public string GetMessages(double latitude, double longitude)
        {
            string userId = this.GenerateUserId();
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();

            RegisterUser(latitude, longitude, userId, salt);
            return GetMessages(latitude, longitude, userId, salt);
        }

        /// <summary>
        /// Get messages using an existing userid
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public string GetMessages(double latitude, double longitude, string userId)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
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

            string hash = Helpers.Encode(encodeUrl, m_deviceKey);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);
            url = BASE_URL + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return Helpers.ReadWebResponse(response);
        }        

        public string PeekMessages(double latitude, double longitude, string peekId)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            string userId = this.GenerateUserId();

            RegisterUser(latitude, longitude, userId, salt);

            return PeekMessages(latitude, longitude, userId, peekId, salt);
        }

        public string PeekMessages(double latitude, double longitude, string userId, string peekId, string salt)
        {
            string url = PEEK_MESSAGES_URL;
            url = url.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-id}", userId);
            url = url.Replace("{peek-id}", peekId);
            url = url.Replace("{version}", VERSION);
            string encodeUrl = url;
            encodeUrl += salt;

            string hash = Helpers.Encode(encodeUrl, m_deviceKey);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);
            url = BASE_URL + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return Helpers.ReadWebResponse(response);
        }

        public string Yaks(double latitude, double longitude, double userLatitude, double userLongitude)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            string userId = this.GenerateUserId();

            RegisterUser(latitude, longitude, userId, salt);

            return Yaks(latitude, longitude, userId, userLatitude, userLongitude, salt);
        }

        public string Yaks(double latitude, double longitude, string userId, double userLatitude, double userLongitude)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            return Yaks(latitude, longitude, userId, userLatitude, userLongitude, salt);
        }

        public string Yaks(double latitude, double longitude, string userId, double userLatitude, double userLongitude, string salt)
        {
            string url = YAKS_URL;
            url = url.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-id}", userId);
            url = url.Replace("{user-latitude}", userLatitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-longitude}", userLongitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{version}", VERSION);
            string encodeUrl = url;
            encodeUrl += salt;

            string hash = Helpers.Encode(encodeUrl, m_deviceKey);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);
            url = BASE_URL + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;
            request.Method = "GET";
            request.UserAgent = USER_AGENT;
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            return Helpers.ReadWebResponse(response);
        }
        
        public Task<string> YaksAsync(double latitude, double longitude, string userId, double userLatitude, double userLongitude)
        {
            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            return YaksAsync(latitude, longitude, userId, userLatitude, userLongitude, salt);
        }

        public async Task<string> YaksAsync(double latitude, double longitude, string userId, double userLatitude, double userLongitude, string salt)
        {
            string url = YAKS_URL;
            //string url = GET_MESSAGES_URL;
            url = url.Replace("{latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-id}", userId);
            //url = url.Replace("{user-latitude}", userLatitude.ToString(CultureInfo.InvariantCulture));
            //url = url.Replace("{user-longitude}", userLongitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-latitude}", latitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{user-longitude}", longitude.ToString(CultureInfo.InvariantCulture));
            url = url.Replace("{version}", VERSION);
            url = url + "&bc=0";
            string encodeUrl = url;
            encodeUrl += salt;

            string hash = Helpers.Encode(encodeUrl, m_deviceKey);
            url = url + "&salt={salt}".Replace("{salt}", salt);
            url = url + "&hash={hash}".Replace("{hash}", hash);            
            url = BASE_URL + url;

            HttpWebRequest request = WebRequest.Create(url) as HttpWebRequest;            
            request.Method = "GET";
            request.UserAgent = USER_AGENT;

            // ** Set a proxy if configured
            if (string.IsNullOrEmpty(m_proxy_address) == false)            
                request.Proxy = new WebProxy(m_proxy_address);            

            var response = await request.GetResponseAsync();

            var content = await Helpers.ReadWebResponseAsync(response);

            return content;
        }
    }
}
