using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public class SoccerVistaInfo
    {
        public string country { get; set; }
        public string league { get; set; }
        public string leagueUrl { get; set; }
        public double homewin { get; set; }
        public double homedraw { get; set; }
        public double homelost { get; set; }
        public double percent { get; set; }
        public List<MatchInfo> matchInfos = new List<MatchInfo>();
    }
}
