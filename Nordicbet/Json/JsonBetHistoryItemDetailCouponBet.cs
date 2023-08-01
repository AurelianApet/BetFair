using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistoryItemDetailCouponBet
    {
        public double stake { get; set; }
        public double taxAmount { get; set; }
        public double stakeForReview { get; set; }
        public List<JsonBetHistoryItemDetailCouponBetBetselection> betSelections { get; set; }
        public int status { get; set; }
        public int merchantBetId { get; set; }
        public int betType { get; set; }
        public object potentialPayout { get; set; }
        public double payout { get; set; }
    }
}
