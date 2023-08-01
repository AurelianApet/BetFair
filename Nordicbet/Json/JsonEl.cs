using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonEl
    {
        public long ei { get; set; }
        public object eit { get; set; }
        public string en { get; set; }
        public bool il { get; set; }
        public string sd { get; set; }
        public int cep { get; set; }
        public int et { get; set; }
        public int ci { get; set; }
        public string cn { get; set; }
        public int ri { get; set; }
        public string rn { get; set; }
        public int sci { get; set; }
        public string scn { get; set; }
        public JsonElSr sr { get; set; }
        public List<JsonElEpl> epl { get; set; }
        public int mc { get; set; }
        public List<JsonElMl> ml { get; set; }
        public object[] sl { get; set; }
        public object ss { get; set; }
        public object sb { get; set; }
    }
}
