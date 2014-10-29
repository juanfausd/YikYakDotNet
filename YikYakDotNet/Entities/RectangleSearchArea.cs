using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YikYakDotNet.Statistics;

namespace YikYakDotNet.Entities
{
    public class RectangleSearchArea
    {
        public double MinLongitude { get; set; }
        public double MaxLongitude { get; set; }
        public double MinLatitude { get; set; }
        public double MaxLatitude { get; set; }

        public List<LocationPoint> Points { get; set; }

        public RectangleSearchArea() 
        {
            Points = new List<LocationPoint>();
        }

        public RectangleSearchArea(List<LocationPoint> points)
        {
            Points = points;
        }

        public LocationPoint CalculateCenter()
        {
            if (Points == null || Points.Count == 0)
                return null;

            MinLongitude = Points[0].Longitude;
            MaxLongitude = Points[0].Longitude;
            MinLatitude = Points[0].Latitude;
            MaxLatitude = Points[0].Latitude;

            for (int i = 1; i < Points.Count; i++)
            {
                if (Points[i].Longitude < MinLongitude)
                    MinLongitude = Points[i].Longitude;

                if (Points[i].Longitude > MaxLongitude)
                    MaxLongitude = Points[i].Longitude;

                if (Points[i].Latitude < MinLatitude)
                    MinLatitude = Points[i].Latitude;

                if (Points[i].Latitude > MaxLatitude)
                    MaxLatitude = Points[i].Latitude;
            }

            double width = MaxLongitude - MinLongitude;
            double height = MaxLatitude - MinLatitude;
            double latitude = MinLatitude + height / 2;
            double longitude = MinLongitude + width / 2;

            return new LocationPoint(latitude, longitude);
        }
    }
}
