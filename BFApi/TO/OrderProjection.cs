using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace BFApi.TO
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum OrderProjection
    {
        ALL, EXECUTABLE, EXECUTION_COMPLETE
    }
}
