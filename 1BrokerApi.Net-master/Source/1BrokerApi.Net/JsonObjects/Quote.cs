using Newtonsoft.Json;
using System;

namespace Jojatekok.OneBrokerAPI.JsonObjects
{
    public class Quote
    {
        [JsonProperty("symbol")]
        public string Symbol { get; private set; }

        [JsonProperty("bid")]
        public decimal MarketBid { get; private set; }

        [JsonProperty("ask")]
        public decimal MarketAsk { get; private set; }

        public decimal Spread {
            get { return Math.Round(MarketAsk - MarketBid, 8); }
        }

        [JsonProperty("updated")]
        public DateTime TimeUpdated { get; private set; }
    }
}
