using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YikYakDotNet.Entities;

namespace YikYakDotNet.Statistics
{
    public class StatisticsManager : IStatisticsManager
    {
        /// <summary>
        /// Get Yaks and a potential list of locations.
        /// </summary>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="numIterations"></param>
        /// <returns></returns>
        public List<GetMessagesResult> GetLocationVariations(double latitude, double longitude, int numIterations)
        {
            YikYakAPI api = new YikYakAPI();

            string salt = Helpers.ConvertToUnixTimestamp(DateTime.Now).ToString();
            //string userId = Helpers.CalculateMD5Hash(Guid.NewGuid().ToString());
            string userId = Guid.NewGuid().ToString().ToUpper();
            api.RegisterUser(latitude, longitude, userId, salt);

            List<GetMessagesResult> result = new List<GetMessagesResult>();

            for(int i=0; i<numIterations; i++)
            {
                try
                {
                    Random ran = new Random();
                    double sign = ran.Next(-1, 1);
                    sign = sign / (int)Math.Abs(sign);
                    double delta = ran.NextDouble() * 0.2 * sign;
                    double ranLatitude = latitude + delta;
                    List<Yak> yaks = api.GetYaks(latitude, longitude, userId, salt);

                    foreach (Yak yak in yaks)
                    {
                        GetMessagesResult existentResult = result.Where(r => r.MessageID.Equals(yak.MessageID)).FirstOrDefault();

                        if (existentResult == null)
                        {
                            existentResult = new GetMessagesResult();
                            existentResult.MessageID = yak.MessageID;
                            existentResult.Message = yak.Message;
                            result.Add(existentResult);
                        }

                        existentResult.Locations.Add(new LocationPoint(yak.Latitude, yak.Longitude));
                    }
                }
                catch (Exception) { }
            }

            return result;
        }
    }
}
