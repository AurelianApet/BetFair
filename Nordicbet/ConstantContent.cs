using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Nordicbet
{
    public static class ConstantContent
    {
        public static string Url = "https://api.betfair.com/exchange/betting";

        public static string getMarketIdFromUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            GroupCollection groups = Regex.Match(url, "\\?id=(?<id>\\d+\\.\\d+)").Groups;
            if (groups == null || groups["id"] == null)
                return string.Empty;

            return groups["id"].Value;
        }

        public static string getRandomString()
        {
            return Guid.NewGuid().ToString().Replace("-", string.Empty);
        }
    }
}
