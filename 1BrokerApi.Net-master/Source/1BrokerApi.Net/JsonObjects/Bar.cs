using Newtonsoft.Json;
using System;

namespace Jojatekok.OneBrokerAPI.JsonObjects
{
    public class Bar
    {
        [JsonProperty("time")]
        public string Time { get; private set; }

        [JsonProperty("o")]
        public decimal o { get; private set; }

        [JsonProperty("h")]
        public decimal h { get; private set; }

        [JsonProperty("l")]
        public decimal l { get; private set; }

        [JsonProperty("c")]
        public decimal c { get;  private set; }
        
    }
}
