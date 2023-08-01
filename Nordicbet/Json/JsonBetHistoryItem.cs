using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistoryItem
    {
        public long couponId { get; set; }
        public string couponArriveDate { get; set; }
        public double stake { get; set; }
        public double taxAmount { get; set; }
        public int betStatus { get; set; }
        public bool isLive { get; set; }
        public bool isMobile { get; set; }
        public bool isBonus { get; set; }
    }
}
