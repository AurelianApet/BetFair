using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet
{
    public class ContentTypes
    {
        private string _type;
        public static readonly ContentTypes FormUrlencoded = new ContentTypes("application/x-www-form-urlencoded");
        public static readonly ContentTypes Json = new ContentTypes("application/json; charset=UTF-8");

        private ContentTypes(string type)
        {
            this._type = type;
        }

        public override string ToString()
        {
            return this._type;
        }
    }
}
