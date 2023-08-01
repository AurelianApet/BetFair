using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Nordicbet.Json;

namespace Nordicbet
{
    public class LoginResult
    {
        public bool success { get; set; }
        public List<JsonElExt> el { get; set; }
        public string error { get; set; }

        public LoginResult()
        {
            success = false;
            el = new List<JsonElExt>();
            error = "unknown error!";
        }
    }
}
