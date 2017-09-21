using Newtonsoft.Json;
using System;

namespace Jojatekok.OneBrokerAPI.JsonObjects
{
    public class Order
    {
        [JsonProperty("order_id")]
        public ulong Id { get; set; }

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

        [JsonProperty("order_type")]
        private string TypeInternal {
            set {
                switch (value) {
                    case "Market":
                        Type = OrderType.Market;
                        break;

                    case "Limit":
                        Type = OrderType.Limit;
                        break;

                    case "Stopentry":
                        Type = OrderType.StopEntry;
                        break;

                    default:
                        Type = OrderType.Unsupported;
                        break;
                }
            }
        }
        public OrderType Type { get; private set; }

        [JsonProperty("order_type_parameter")]
        public decimal TypeParameter { get; private set; }

        [JsonProperty("stop_loss")]
        public decimal? StopLoss { get; private set; }

        [JsonProperty("take_profit")]
        public decimal? TakeProfit { get; private set; }

        [JsonProperty("created")]
        public DateTime TimeCreated { get; private set; }

        public Order(string symbol, decimal amountMargin, TradeDirection tradeDirection, float leverage, OrderType type, decimal typeParameter = -1, decimal? stopLoss = null, decimal? takeProfit = null)
        {
            Symbol = symbol;
            AmountMargin = amountMargin;
            TradeDirection = tradeDirection;
            Leverage = leverage;
            Type = type;
            TypeParameter = typeParameter;
            StopLoss = stopLoss;
            TakeProfit = takeProfit;
        }
    }
}
