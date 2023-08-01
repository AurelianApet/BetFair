using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance
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
        public string domain { get; set; }
        public int delay { get; set; }

        public Settings()
        {
            username = string.Empty;
            password = string.Empty;
            domain = string.Empty;
            delay = 0;
        }
    }
}
