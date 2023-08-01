using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public class Settings
    {
        private static Settings _instance = null;

        public static Settings Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new Settings();
                }

                return _instance;
            }
        }

        public string username { get; set; }
        public string password { get; set; }
        public bool useSideOddsMin { get; set; }
        public double sideOddsMin { get; set; }
        public bool useSideOddsMax { get; set; }
        public double sideOddsMax { get; set; }
        public int sideDelta { get; set; }
        public string currency { get; set; }
        public bool useProgression { get; set; }
        public int stopStep { get; set; }
        public int betMax { get; set; }
        public double flatStake { get; set; }
        public List<double> stakes { get; set; }
        public bool useFilter { get; set; }
        public string delayKey { get; set; }
        public string activeKey { get; set; }
        public double percentBalance { get; set; }
        public bool useFixedStake { get; set; }
        public bool usePercent { get; set; }

        public Settings()
        {
            username = string.Empty;
            password = string.Empty;
            useSideOddsMin = true;
            sideOddsMin = 0;
            useSideOddsMax = true;
            sideOddsMax = 0;
            sideDelta = 0;
            currency = string.Empty;
            useProgression = false;
            useFilter = false;
            stopStep = 0;
            betMax = 10;
            flatStake = 0;
            stakes = new List<double>();
            percentBalance = 0;
            useFixedStake = true;
            usePercent = false;
        }
    }
}
