using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet.Statistics
{
    public class GetMessagesResult
    {
        public string MessageID { get; set; }
        public string Message { get; set; }
        public List<LocationPoint> Locations { get; set; }

        public GetMessagesResult()
        {
            Locations = new List<LocationPoint>();
        }

        public double MaxDistanceInDegrees
        {
            get
            {
                double maxDistance = 0;
                var avgLocation = AvgLocation;

                foreach (var point in Locations)
                {
                    double distance = Math.Sqrt(Math.Pow((point.Latitude - avgLocation.Latitude), 2) + Math.Pow((point.Longitude - avgLocation.Longitude), 2));

                    if (distance > maxDistance)
                        maxDistance = distance;
                }

                return maxDistance;
            }
        }

        public LocationPoint AvgLocation
        {
            get
            {
                double avgLatitude = Locations.Select(l => l.Latitude).Average();
                double avgLongitude = Locations.Select(l => l.Longitude).Average();

                return new LocationPoint(avgLatitude, avgLongitude);
            }
        }
    }
}
