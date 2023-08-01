using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public static class Constants
    {
        public static string[] currency = new string[]
        {
            "AUD", 
            "CAD",
            "RMB",
            "CZK",
            "EUR",
            "HKD",
            "JPY",
            "MYR",
            "MXP",
            "NZD",
            "NOK",
            "PLN",
            "RUB",
            "KRW",
            "SEK",
            "TWD",
            "THB",
            "GBP",
            "USD"
        };

        public static int getCurrencyIndex(string cur)
        {
            int nIndex = 0;
            foreach(string c in currency)
            {
                if (c == cur)
                    return nIndex;

                nIndex++;
            }

            return -1;
        }

        public static int step = 0;
        public static DateTime start = DateTime.Now;

        private static string[] betStatus = new string[]{
            "", "Open", "Won", "Lost", "Void", "HalfWon", "HalfLost", "Rejected", "Pending", "PartGranted"
        };

        public static string getBetStatus(int status)
        {
            if (status < 0)
                return string.Empty;

            if (status < betStatus.Length)
                return betStatus[status];
            else
                return string.Empty;
        }
    }
}
