using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YikYakDotNet.Statistics
{
    public interface IStatisticsManager
    {
        List<GetMessagesResult> GetLocationVariations(double latitude, double longitude, int numIterations);
    }
}
