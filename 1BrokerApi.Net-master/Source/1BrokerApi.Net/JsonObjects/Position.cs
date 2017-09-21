using Newtonsoft.Json;

namespace Jojatekok.OneBrokerAPI.JsonObjects
{
    public class Position
    {
        [JsonProperty("position_id")]
        public ulong Id { get; private set; }

        [JsonProperty("status")]
        private string StatusInternal {
            set { Status = value == "open" ? PositionStatus.Open : PositionStatus.Closed; }
        }
        public PositionStatus Status { get; private set; }

        [JsonProperty("symbol")]
        public string Symbol { get; private set; }

        [JsonProperty("margin")]
        public decimal AmountMargin { get; private set; }

        [JsonProperty("leverage")]
        public float Leverage { get; private set; }

        [JsonProperty("direction")]
        private string TradeDirectionInternal {
            set { TradeDirection = value == "long" ? TradeDirection.Long : TradeDirection.Short; }
        }
        public TradeDirection TradeDirection { get; private set; }

        [JsonProperty("entry_price")]
        public decimal PriceEntry { get; private set; }

        [JsonProperty("current_bid")]
        public decimal SymbolMarketBid { get; private set; }

        [JsonProperty("current_ask")]
        public decimal SymbolMarketAsk { get; private set; }

        [JsonProperty("profit_loss")]
        public decimal ProfitOrLossAmount { get; private set; }

        [JsonProperty("profit_loss_percent")]
        public decimal ProfitOrLossPercentage { get; private set; }

        [JsonProperty("market_close")]
        public bool IsClosedWithMarketOrder { get; private set; }

        [JsonProperty("stop_loss")]
        public decimal? StopLoss { get; private set; }

        [JsonProperty("take_profit")]
        public decimal? TakeProfit { get; private set; }
    }
}
