using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public class MatchInfo
    {
        public string date { get; set; }
        public int round { get; set; }
        public string homeTeam { get; set; }
        public string awayTeam { get; set; }
        public string resultPrediction { get; set; }
        public string resultTip { get; set; }
        public string resultPick { get; set; }
        public string eventName { get; set; }
        public double homeWin { get; set; }
        public double homeDraw { get; set; }
        public double homeLost { get; set; }
    }
}
