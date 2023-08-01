using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonElExt
    {
        public JsonEl el { get; set; }
        public double diff { get; set; }

        public JsonElExt()
        {
            el = new JsonEl();
            diff = 0;
        }

        public JsonElExt(JsonEl _el, double _diff)
        {
            el = _el;
            diff = _diff;
        }
    }
}
