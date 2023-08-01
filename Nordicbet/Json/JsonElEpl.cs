using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonElEpl
    {
        public int pi { get; set; }
        public int so { get; set; }
        public List<JsonElEplSl> sl { get; set; }
        public string pn { get; set; }
    }
}
