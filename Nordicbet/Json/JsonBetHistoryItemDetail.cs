using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistoryItemDetail
    {
        public JsonBetHistoryItemDetailCoupon coupon { get; set; }
        public List<JsonBetHistoryItemDetailMarket> market { get; set; }
        public List<JsonBetHistoryItemDetailSubCategory> subCategory { get; set; }
    }
}
