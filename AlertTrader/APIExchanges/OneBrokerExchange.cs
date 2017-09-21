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

        public OneBrokerExchange()
        {
            client = new OneBrokerClient(Properties.Settings.Default.OneBrokerApiToken);
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

        public decimal GetBalance(string symbol)
        {
            throw new NotImplementedException();
        }

        public decimal GetCurrentPrice(string symbol)
        {
            throw new NotImplementedException();
        }
        
        public decimal Long()
        {
            throw new NotImplementedException();
            //TradeDirection tradeDirection = TradeDirection.Long;
            //client.Orders.PostOrder(new Order(symbol, ammountMargin, tradeDirection, leverage, OrderType.Market));
        }
        
        public decimal Short()
        {
            //TradeDirection tradeDirection = TradeDirection.Short;
            //Order order = client.Orders.PostOrder(new Order(symbol, ammountMargin, tradeDirection, leverage, OrderType.Market));
            throw new NotImplementedException();
        }
    }
}
