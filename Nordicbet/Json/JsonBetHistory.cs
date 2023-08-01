using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistory
    {
        public List<JsonBetHistoryItem> betHistoryItems { get; set; }
        public int numberOfCoupons { get; set; }
        public int numberOfPages { get; set; }
    }
}
