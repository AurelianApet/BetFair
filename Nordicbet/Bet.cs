using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public class Bet
    {
        public string time { get; set; }
        public string league { get; set; }
        public string match { get; set; }
        public double odds { get; set; }
        public double stake { get; set; }
        public string status { get; set; }

        public Bet()
        {
            time = string.Empty;
            league = string.Empty;
            match = string.Empty;
            odds = 0;
            stake = 0;
            status = string.Empty;
        }
    }
}
