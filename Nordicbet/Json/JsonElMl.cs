using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonElMl
    {
        public long mi { get; set; }
        public object mit { get; set; }
        public object mti { get; set; }
        public string mn { get; set; }
        public string dd { get; set; }
        public string ht { get; set; }
        public int bgi { get; set; }
        public string bgn { get; set; }
        public string bgd { get; set; }
        public int bgso { get; set; }
        public int bgti { get; set; }
        public int bgtci { get; set; }
        public int bggi { get; set; }
        public string bggn { get; set; }
        public int bggso { get; set; }
        public int cri { get; set; }
        public string lv { get; set; }
        public List<JsonElMlMsl> msl { get; set; }
        public int ms { get; set; }
        public bool iel { get; set; }
        public int lvt { get; set; }
        public double lvrv { get; set; }
        public object mp { get; set; }
    }
}
