﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;


namespace BFApi.TO
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum GroupBy
    {
        EVENT_TYPE,
        EVENT,
        MARKET,
        SIDE,
        BET
    }
}
