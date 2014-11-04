using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet.Entities
{
    public class Location
    {
        public string PeekID { get; set; }
        public string Name { get; set; }
        public bool Inactive { get; set; }
        public bool CanSubmit { get; set; }
        public bool CanVote { get; set; }
        public bool CanReply { get; set; }
        public bool CanReport { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Delta { get; set; }
    }
}
