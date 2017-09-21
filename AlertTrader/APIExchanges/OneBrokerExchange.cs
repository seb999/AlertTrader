using System;
using System.Linq;
using Jojatekok.OneBrokerAPI;
using Jojatekok.OneBrokerAPI.JsonObjects;
using Jojatekok.OneBrokerAPI.ClientTools;
using System.Collections.Generic;

namespace AlertTrader.APIExchanges
{
    public class OneBrokerExchange : IAPIExchange
    {
        OneBrokerClient client;

        public OneBrokerExchange(string apiToken)
        {
            client = new OneBrokerClient(apiToken);
            if (!client.IsApiTokenValid())
            {
                throw new Exception("API token is not valid");
            }
        }

        public void CloseAllPositions()
        {
            List<Position> openPositions = client.Positions.GetOpenPositions().ToList();
            foreach(var pos in openPositions)
            {
                client.Positions.EditPosition(pos.Id);
            }
        }

        public void CloseAllpositions()
        {
            throw new NotImplementedException();
        }

        public void GetBalance(string symbol)
        {
           
        }

        public decimal GetCurrentPrice()
        {
            throw new NotImplementedException();
        }
        
        public void Long(string symbol, decimal ammountMargin, int leverage)
        {
            TradeDirection tradeDirection = TradeDirection.Long;
            client.Orders.PostOrder(new Order(symbol, ammountMargin, tradeDirection, leverage, OrderType.Market));
        }

        public bool Long()
        {
            throw new NotImplementedException();
        }

        public void Short(string symbol, decimal ammountMargin, int leverage)
        {
            TradeDirection tradeDirection = TradeDirection.Short;
            Order order = client.Orders.PostOrder(new Order(symbol, ammountMargin, tradeDirection, leverage, OrderType.Market));
        }

        public bool Short()
        {
            throw new NotImplementedException();
        }
    }
}
