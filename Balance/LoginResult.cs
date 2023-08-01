using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Balance
{
    public class LoginResult
    {
        public bool success { get; set; }
        public double balance { get; set; }
        public double bonus { get; set; }
        public string currency { get; set; }
        public string currencyBonus { get; set; }

        public LoginResult()
        {
            success = false;
            balance = 0;
            currency = string.Empty;
        }
    }
}
