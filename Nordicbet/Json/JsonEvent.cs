using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonEvent
    {
        public int tec { get; set; }
        public int tp { get; set; }
        public int pn { get; set; }
        public List<JsonEl> el { get; set; }
    }
}
