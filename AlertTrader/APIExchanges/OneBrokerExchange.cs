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

        public decimal GetBalance(string symbol)
        {
            return decimal.Parse(client.Account.GetAccountInfo().BalanceInBitcoins);
        }

        public decimal GetCurrentPrice(string baseCurrency, string market)
        {
            string[] quotes = new string[1] { market };
            return client.Markets.GetQuotes(quotes).First().MarketAsk;
        }

        public decimal Long()
        {
            try
            {
                string baseCurrency = Properties.Settings.Default.BaseCurrency;
                string market = Properties.Settings.Default.Market;
                int leverage = int.Parse(Properties.Settings.Default.Leverage);

                decimal balance = this.GetBalance(baseCurrency);
                decimal price = this.GetCurrentPrice(baseCurrency,market);

                decimal ammountMargin;
                if (Properties.Settings.Default.UsingFixedAmmount)
                {
                    ammountMargin = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammountMargin = Properties.Settings.Default.CapitalPercentageInEachOrder * (balance / price);
                }

                TradeDirection tradeDirection = TradeDirection.Long;

                client.Orders.PostOrder(new Order(market, ammountMargin, tradeDirection, leverage, OrderType.Market));

                return price;
            }
            catch (Exception ex)
            {
                return 0;
            }
        }

        public decimal Short()
        {
            try
            {
                string baseCurrency = Properties.Settings.Default.BaseCurrency;
                string market = Properties.Settings.Default.Market;
                int leverage = int.Parse(Properties.Settings.Default.Leverage);

                decimal balance = this.GetBalance(baseCurrency);
                decimal price = this.GetCurrentPrice(baseCurrency,market);

                decimal ammountMargin;
                if (Properties.Settings.Default.UsingFixedAmmount)
                {
                    ammountMargin = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammountMargin = Properties.Settings.Default.CapitalPercentageInEachOrder * (balance / price);
                }

                TradeDirection tradeDirection = TradeDirection.Short;

                client.Orders.PostOrder(new Order(market, ammountMargin, tradeDirection, leverage, OrderType.Market));

                return price;
            }
            catch (Exception ex)
            {

            }
            return 0;
        }
    }
}
