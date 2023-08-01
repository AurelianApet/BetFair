using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistoryItemDetailCouponBetBetselection
    {
        public long marketSelectionId { get; set; }
        public double odds { get; set; }
        public long marketId { get; set; }
        public int status { get; set; }
        public int voidReasonID { get; set; }
        public object voidReasonText { get; set; }
    }
}
