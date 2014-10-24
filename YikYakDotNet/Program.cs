using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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

            double latitude = 38.832194;
            double longitude = -77.308037;

            try
            {
                string res = api.GetMessages(latitude, longitude);

                var deserializerSettings = new JsonSerializerSettings()
                {
                    DateFormatHandling = DateFormatHandling.IsoDateFormat,
                    DateParseHandling = Newtonsoft.Json.DateParseHandling.None,
                    DateTimeZoneHandling = Newtonsoft.Json.DateTimeZoneHandling.Utc
                };

                JToken token = JsonConvert.DeserializeObject<JObject>(res, deserializerSettings);

                Console.WriteLine(string.Format("Total Results: {0}", token["messages"].Count()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
