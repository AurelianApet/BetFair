﻿using System;
using System.Linq;
using Newtonsoft.Json;

namespace BFApi.TO
{

    public class MarketTypeResult
    {
        [JsonProperty(PropertyName = "marketType")]
        public string marketType { get; set; }

        [JsonProperty(PropertyName = "marketCount")]
        public int marketCount { get; set; }
    }
}
