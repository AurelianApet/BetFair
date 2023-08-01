using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistoryItemDetailCoupon
    {
        public List<JsonBetHistoryItemDetailCouponBet> bets { get; set; }
        public object bonusCustomerId { get; set; }
        public bool isForManualAttest { get; set; }
        public long couponId { get; set; }
    }
}
