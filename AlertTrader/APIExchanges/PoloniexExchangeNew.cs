//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using XCT.BaseLib.API.Poloniex;
//using XCT.BaseLib.API.Poloniex.Public;
//using XCT.BaseLib.API.Poloniex.Trade;
//using XCT.BaseLib.API.Poloniex.User;
//using XCT.BaseLib.Types;

//namespace AlertTrader.APIExchanges
//{
//    public class PoloniexExchange : IAPIExchange
//    {
//        PPublicApi publicApi;
//        PUserApi api;
//        PTradeApi tradeApi;
        
//        public PoloniexExchange()
//        {
//            publicApi = new PPublicApi();
//            api = new PUserApi(Properties.Settings.Default.PoloniexApiKey, Properties.Settings.Default.PoloniexApiSecret);
//            tradeApi = new PTradeApi(Properties.Settings.Default.PoloniexApiKey, Properties.Settings.Default.PoloniexApiSecret);
//        }

//        public decimal GetBalance(string symbol)
//        {
//            return GetBalanceTask(symbol).Result;
//        }
//        public decimal GetCurrentPrice(string symbol)
//        {
//            return GetCurrentPriceTask(symbol).Result;
//        }

//        public async Task<decimal> GetBalanceTask(string symbol)
//        {
//            var balances = await api.GetBalances();
//            return balances[symbol].BitcoinValue;
//        }

//        public async Task<decimal> GetCurrentPriceTask(string symbol)
//        {
//            var ticker = await publicApi.GetTicker();
//            return ticker[symbol].PriceLast;
//        }

//        public decimal Long()
//        {
//            return LongTask().Result;
//        }
//        public decimal Short()
//        {
//            return ShortTask().Result;
//        }

//        public async Task<decimal> LongTask()
//        {
//            try
//            {
//                string baseCurrency = Properties.Settings.Default.BaseCurrency;
//                string market = Properties.Settings.Default.Market;
//                int leverage = int.Parse(Properties.Settings.Default.Leverage);

//                decimal balance = this.GetBalance("BTC");
//                decimal price = this.GetCurrentPrice(market);

//                decimal ammount;
//                if (Properties.Settings.Default.UsingFixedAmmount)
//                {
//                    ammount= Convert.ToDecimal(Properties.Settings.Default.FixedAmmount);
//                }
//                else
//                {
//                    ammount= Convert.ToDecimal(Properties.Settings.Default.CapitalPercentageInEachOrder) * (balance / price);
//                }

//                CurrencyPair pair = new CurrencyPair(baseCurrency, market);
//                decimal limitBuyPrice = price * ((100 + Properties.Settings.Default.LimitSpreadPercentage) / 100);

//                ulong orderId = tradeApi.PlaceOrder(pair, OrderType.Buy, limitBuyPrice, ammount).Result;
                
//                //===========================================================
                
//                return Convert.ToDecimal(price);
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//        }


//        public async Task<decimal> ShortTask()
//        {
//            try
//            {
//                string baseCurrency = Properties.Settings.Default.BaseCurrency;
//                string market = Properties.Settings.Default.Market;
//                int leverage = int.Parse(Properties.Settings.Default.Leverage);

//                decimal balance = this.GetBalance("BTC");
//                decimal price = this.GetCurrentPrice(market);

//                decimal ammount;
//                if (Properties.Settings.Default.UsingFixedAmmount)
//                {
//                    ammount = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
//                }
//                else
//                {
//                    ammount = Convert.ToDecimal(Properties.Settings.Default.CapitalPercentageInEachOrder) * (balance / price);
//                }

//                CurrencyPair pair = new CurrencyPair(baseCurrency, market);
//                decimal limitSellPrice = price * ((100 - Properties.Settings.Default.LimitSpreadPercentage) / 100);
                
//                ulong orderId = tradeApi.PlaceOrder(pair, OrderType.Buy, limitSellPrice, ammount).Result;

//                //===========================================================

//                return Convert.ToDecimal(price);
//            }
//            catch (Exception ex)
//            {
//                return 0;
//            }
//        }
//    }
//}
