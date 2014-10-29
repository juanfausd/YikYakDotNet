using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet.Statistics
{
    public class LocationPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public LocationPoint(double latitude, double longitude)
        {
            Latitude = latitude;
            Longitude = longitude;
        }
    }
}
