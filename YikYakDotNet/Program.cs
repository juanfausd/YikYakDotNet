using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YikYakDotNet.Entities;
using YikYakDotNet.Statistics;

namespace YikYakDotNet
{
    class Program
    {
        // This was calculated while running several tests
        public static double MAX_DISTANCE_IN_METERS = 2222.4; // 0.02 * 111.12 * 1000

        static void Main(string[] args)
        {
            // Santa Fe, Argentina
            //double latitude = -31.641325;
            //double longitude = -60.718780;

            // Emerson College
            double latitude = 34.068672;
            double longitude = -81.096931;
            string peekId = "100288";

            string messageID = "R/54701f957bc83c79bbc87535bf0af";
            LocationPoint knownLocation = new LocationPoint(latitude, longitude);

            GetYaks(latitude, longitude);

            //PeekMessages(latitude, longitude, peekId);

            //PeekAnywhere(latitude, longitude, latitude, longitude);

            //GetLocations(latitude, longitude);

            //GetLocationVariations(latitude, longitude, 100);

            //GetAverageLocationVariations(messageID, knownLocation, 100);
        }

        private static void PeekAnywhere(double latitude, double longitude, double userLatitude, double userLongitude)
        {
            YikYakAPI api = new YikYakAPI();

            try
            {
                var res = api.Yaks(latitude, longitude, userLatitude, userLongitude);

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

        private static void PeekMessages(double latitude, double longitude, string peekId)
        {
            YikYakAPI api = new YikYakAPI();

            try
            {
                var res = api.PeekMessages(latitude, longitude, peekId);

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

        private static void GetYaks(double latitude, double longitude)
        {
            YikYakAPI api = new YikYakAPI();

            try
            {
                var res = api.GetYaks(latitude, longitude);

                Console.WriteLine(string.Format("Total Results: {0}", res.Count()));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static void GetLocations(double latitude, double longitude)
        {
            YikYakAPI api = new YikYakAPI();

            try
            {
                int totalResults = 0;
                string fileName = string.Format(@"Locations_{0}.txt", DateTime.Now.ToString("MMddyyyyHHmmss"));
                FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);

                var res = api.GetFeaturedLocations(latitude, longitude);
                totalResults += res.Count;

                foreach (var item in res)
                {
                    writer.Write(string.Format("{0}\t\t\t{1}\t\t\t\t{2},{3}\t\t{4}", item.PeekID, item.Name, item.Latitude, item.Longitude, item.Delta));
                    writer.WriteLine();
                }

                res = api.GetOtherLocations(latitude, longitude);
                totalResults += res.Count;

                foreach (var item in res)
                {
                    writer.Write(string.Format("{0}\t\t\t{1}\t\t\t\t{2},{3}\t\t{4}", item.PeekID, item.Name, item.Latitude, item.Longitude, item.Delta));
                    writer.WriteLine();
                }

                writer.Close();

                Console.WriteLine(string.Format("Total Results: {0}", totalResults));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static void GetLocationVariations(double latitude, double longitude, int iterationsNum)
        {
            StatisticsManager manager = new StatisticsManager();

            try
            {
                var items = manager.GetLocationVariations(latitude, longitude, iterationsNum);

                string fileName = string.Format(@"LocationVariations_{0}.txt", DateTime.Now.ToString("MMddyyyyHHmmss"));
                FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);

                foreach(var item in items)
                {
                    var avgLocation = item.AvgLocation;
                    double maxDistanceInDegrees = item.MaxDistanceInDegrees;
                    double maxDistanceInKm = maxDistanceInDegrees * 111.12;

                    writer.Write(item.MessageID + "\t\t");
                    foreach (var point in item.Locations)
                    {
                        writer.Write(string.Format("{0},{1}\t", point.Latitude, point.Longitude));
                    }

                    writer.Write(string.Format("AVG: {0}, {1}\t", avgLocation.Latitude, avgLocation.Longitude));
                    writer.Write(string.Format("MAX DISTANCE FROM AVG (DEGREES): {0}\t", maxDistanceInDegrees));
                    writer.Write(string.Format("MAX DISTANCE FROM AVG (KM): {0}\t", maxDistanceInKm));
                    writer.WriteLine();
                    writer.WriteLine();
                }

                writer.Close();

                Console.WriteLine(string.Format("Statistics exported successfully to {0}", fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }

        private static void GetAverageLocationVariations(string knownMessageID, LocationPoint knownLocation, int maxIterationsNum)
        {
            StatisticsManager manager = new StatisticsManager();

            try
            {
                var items = manager.GetLocationVariations(knownLocation.Latitude, knownLocation.Longitude, maxIterationsNum);

                string fileName = string.Format(@"AverageLocation_{0}.txt", DateTime.Now.ToString("MMddyyyyHHmmss"));
                FileStream stream = new FileStream(fileName, FileMode.OpenOrCreate, FileAccess.Write);
                StreamWriter writer = new StreamWriter(stream);

                var item = items.Where(i => i.MessageID == knownMessageID).FirstOrDefault();

                if(item == null)
                {
                    Console.WriteLine("The message wasn't found.");
                    Console.ReadLine();
                    return;
                }

                //double aggregatedLatitude = 0;
                //double aggregatedLongitude = 0;
                RectangleSearchArea rectangle = new RectangleSearchArea();

                for (int i = 0; i < item.Locations.Count; i++)
                {
                    //aggregatedLatitude += item.Locations[i].Latitude;
                    //aggregatedLongitude += item.Locations[i].Longitude;
                    rectangle.Points.Add(item.Locations[i]);

                    int pointCount = i + 1;
                    if (pointCount % 10 == 0 || i == item.Locations.Count)
                    {
                        //LocationPoint avgLocation = new LocationPoint(aggregatedLatitude / pointCount, aggregatedLongitude / pointCount);
                        LocationPoint avgLocation = rectangle.CalculateCenter();

                        double distance = Math.Sqrt(Math.Pow((knownLocation.Latitude - avgLocation.Latitude), 2) + Math.Pow((knownLocation.Longitude - avgLocation.Longitude), 2));
                        double distanceInKm = distance * 111.12;
                        double confidencePercent = Math.Abs((MAX_DISTANCE_IN_METERS - (distanceInKm * 1000))) / MAX_DISTANCE_IN_METERS;

                        writer.Write(string.Format("NUMBER OF POINTS USED: {0}", pointCount));
                        writer.WriteLine();
                        writer.Write(string.Format("AVG: {0}, {1}", avgLocation.Latitude, avgLocation.Longitude));
                        writer.WriteLine();
                        writer.Write(string.Format("DISTANCE FROM KNOWN POINT (DEGREES): {0}", distance));
                        writer.WriteLine();
                        writer.Write(string.Format("DISTANCE FROM KNOWN POINT (KM): {0}", distanceInKm));
                        writer.WriteLine();
                        writer.Write(string.Format("CONFIDENCE (%): {0}", confidencePercent));
                        writer.WriteLine();
                        writer.WriteLine();
                    }
                }

                writer.Close();

                Console.WriteLine(string.Format("Average Location statistics exported successfully to {0}", fileName));
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            Console.ReadLine();
        }
    }
}
