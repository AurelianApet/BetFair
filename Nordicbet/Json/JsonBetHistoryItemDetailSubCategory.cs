using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Nordicbet.Json
{
    public class JsonBetHistoryItemDetailSubCategory
    {
        public long subCategoryID { get; set; }
        public string subCategoryName { get; set; }
        public long categoryId { get; set;}
        public string categoryName { get; set; }
        public string regionName { get; set; }
    }
}
