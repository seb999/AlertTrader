using AlertTrader.IAPIExchanges;
using Jojatekok.PoloniexAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public class PoloniexExchange : IAPIExchange
    {
        PoloniexClient client;

        public PoloniexExchange()
        {
            client = new PoloniexClient(Properties.Settings.Default.PoloniexApiKey, Properties.Settings.Default.PoloniexApiSecret);
        }

        public decimal GetBalance(string symbol)
        {
            return GetBalanceTask(symbol).Result;
        }
        public decimal GetCurrentPrice(string baseCurrency, string market)
        {
            return GetCurrentPriceTask(market).Result;
        }

        public async Task<decimal> GetBalanceTask(string symbol)
        {
            var balances = await client.Wallet.GetBalancesAsync();
            return Convert.ToDecimal(balances.Single(b => b.Key == "BTC").Value.BitcoinValue);
        }

        public async Task<decimal> GetCurrentPriceTask(string symbol)
        {
            var summaries = await client.Markets.GetSummaryAsync();

            decimal price = Convert.ToDecimal(summaries.Single(s => s.Key.QuoteCurrency == symbol).Value.PriceLast);
            return price;
        }

        public decimal Long()
        {
            return LongTask().Result;
        }
        public decimal Short()
        {
            return ShortTask().Result;
        }

        public async Task<decimal> LongTask()
        {
            try
            {
                string baseCurrency = Properties.Settings.Default.BaseCurrency;
                string market = Properties.Settings.Default.Market;
                int leverage = int.Parse(Properties.Settings.Default.Leverage);

                double balance = Convert.ToDouble(this.GetBalance("BTC"));
                double price = Convert.ToDouble(this.GetCurrentPrice(baseCurrency,market));

                double ammount;
                if (Properties.Settings.Default.UsingFixedAmount)
                {
                    ammount = double.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammount = Convert.ToDouble(Properties.Settings.Default.CapitalPercentageInEachOrder) * (balance / price);
                }

                CurrencyPair pair = new CurrencyPair(baseCurrency, market);
                double limitBuyPrice = Convert.ToDouble(price) * ((100 + Properties.Settings.Default.LimitSpreadPercentage) / 100);

                int orderId = client.Trading.PostOrderAsync(pair, OrderType.Buy, ammount, limitBuyPrice).Id;

                //===========================================================

                return Convert.ToDecimal(price);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }


        public async Task<decimal> ShortTask()
        {
            try
            {
                string baseCurrency = Properties.Settings.Default.BaseCurrency;
                string market = Properties.Settings.Default.Market;
                int leverage = int.Parse(Properties.Settings.Default.Leverage);

                double balance = Convert.ToDouble(this.GetBalance("BTC"));
                double price = Convert.ToDouble(this.GetCurrentPrice(baseCurrency,market));

                double ammount;
                if (Properties.Settings.Default.UsingFixedAmount)
                {
                    ammount = double.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammount = Convert.ToDouble(Properties.Settings.Default.CapitalPercentageInEachOrder) * (balance / price);
                }

                CurrencyPair pair = new CurrencyPair(baseCurrency, market);
                double limitSellPrice = Convert.ToDouble(price) * ((100 - Properties.Settings.Default.LimitSpreadPercentage) / 100);

                int orderId = client.Trading.PostOrderAsync(pair, OrderType.Buy, ammount, limitSellPrice).Id;

                //===========================================================

                return Convert.ToDecimal(price);
            }
            catch (Exception ex)
            {
                return 0;
            }
        }
    }
}
