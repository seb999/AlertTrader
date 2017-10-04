using System;
using System.Globalization;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bittrex;
using Newtonsoft.Json.Linq;
using AlertTrader.Misc;
using System.Windows.Media;
using System.Collections.ObjectModel;
using AlertTrader.Classes;
using AlertTrader.IAPIExchanges;
using static AlertTrader.Classes.LookupData;

namespace AlertTrader.APIExchanges
{
    public class BittrexExchange : IAPIExchange
    {
        Exchange exchange;
        decimal totalFees = 0;
        decimal lastBuyPrice = 0;
        decimal totalProfit = 0;

        public BittrexExchange()
        {
            exchange = new Exchange();
            exchange.Initialise(new ExchangeContext()
            {
                ApiKey = Properties.Settings.Default.BittrexApiKey,
                Secret = Properties.Settings.Default.BittrexApiSecret,
                Simulate = false
            });
            
        }

        public decimal Long()
        {
            decimal price = 0;
            try
            {
                string baseCurrency = Properties.Settings.Default.BaseCurrency;
                string market = Properties.Settings.Default.Market;
                
                AccountBalance accountBalance = exchange.GetBalance(baseCurrency);
                decimal balance = accountBalance.Available;
                
                JObject ticker = exchange.GetTicker(market);
                price = (decimal)ticker["Last"];

                decimal ammount;
                if (Properties.Settings.Default.UsingFixedAmount)
                {
                    ammount = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammount = Properties.Settings.Default.CapitalPercentageInEachOrder * (balance / price);
                }

                decimal limitBuyPrice = price * ((100 + Properties.Settings.Default.LimitSpreadPercentage) / 100);

                OrderResponse response = exchange.PlaceBuyOrder(market, ammount, limitBuyPrice);

                string orderId = response.uuid;

                //===============================================

                CompletedOrder order = null;
                bool available = false;

                while (!available)
                {
                    try
                    {
                        GetOrderHistoryResponse historyResponse = exchange.GetOrderHistory("ETH", 1);
                        order = historyResponse.Single(r => r.OrderUuid == orderId);
                        available = true;
                    }
                    catch (Exception e)
                    {
                    }
                }

                if (order.GetType().Name != "CompletedOrder")
                {
                    helper.DisplayUserMessage(string.Format("Order not completed... something went wrong :/"), LookupData.MessageList, Brushes.DarkMagenta);
                    return 0;
                }
                else
                {
                    helper.DisplayUserMessage(string.Format("Limit BUY order executed on {0} id = {1}", market, order.OrderUuid), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Market Price: {0}", price), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Total Order Price: {0}", order.Price), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Unit Price: {0}", order.PricePerUnit), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity: {0}", order.Quantity), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity remaining: {0}", order.QuantityRemaining), LookupData.MessageList, Brushes.Green);

                    decimal spread = 100 - Math.Abs((Math.Round((Decimal.Divide((order.Price - price), price) * 100), 4)));

                    helper.DisplayUserMessage(string.Format("Spread: {0}%", spread), LookupData.MessageList, Brushes.White);
                    helper.DisplayUserMessage(string.Format("Commission: {0}", order.Commission), LookupData.MessageList, Brushes.White);

                    //update price, to return real price per unit
                    price = order.PricePerUnit;
                    totalFees += order.Commission;
                }

            }
            catch (Exception ex)
            {
                helper.DisplayUserMessage(string.Format("Error on Buy Order:"), LookupData.MessageList, Brushes.Red);

                if (ex.Message.Contains("QUANTITY_NOT_PROVIDED"))
                {
                    helper.DisplayUserMessage(string.Format("Quantity/ammount of {0} not valid."), LookupData.MessageList, Brushes.Red);
                }
                else if (ex.Message.Contains("APIKEY_INVALID"))
                {
                    helper.DisplayUserMessage(string.Format("API / KEY INVALID"), LookupData.MessageList, Brushes.Red);
                }
                else
                {
                    helper.DisplayUserMessage(string.Format("ex.Message"), LookupData.MessageList, Brushes.Red);
                    helper.DisplayUserMessage(ex.InnerException.ToString(), LookupData.MessageList, Brushes.Red);
                    helper.DisplayUserMessage(ex.StackTrace.ToString(), LookupData.MessageList, Brushes.Red);
                }
            }
            return price;
        }

        public decimal Short()
        {
            decimal price = 0;

            try
            {
                string baseCurrency = Properties.Settings.Default.BaseCurrency;
                string market = Properties.Settings.Default.Market;

                Exchange exchange = new Exchange();

                decimal balance = this.GetBalance(market);

                helper.DisplayUserMessage(string.Format("Balance: {0}", balance), LookupData.MessageList, Brushes.White);

                JObject ticker = exchange.GetTicker(market);

                price = (decimal)ticker["Last"];

                helper.DisplayUserMessage(string.Format("Price: {0}", price), LookupData.MessageList, Brushes.White);

                decimal ammount;
                if (Properties.Settings.Default.UsingFixedAmount)
                {
                    ammount = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammount = Properties.Settings.Default.CapitalPercentageInEachOrder * (balance / price);
                }

                //decimal limitSellPrice = price * ((100 - limitSpreadPercentage) / 100);
                decimal limitSellPrice = price * (decimal.Divide((100 - Properties.Settings.Default.LimitSpreadPercentage), 100));

                ammount = ammount * (100 - 0.26m) / 100;

                OrderResponse response = exchange.PlaceSellOrder(market, ammount, limitSellPrice);

                string orderId = response.uuid;
                //===============================================

                CompletedOrder order = null;
                bool available = false;

                while (!available)
                {
                    try
                    {
                        GetOrderHistoryResponse historyResponse = exchange.GetOrderHistory("ETH", 1);
                        order = historyResponse.Single(r => r.OrderUuid == orderId);
                        available = true;
                    }
                    catch (Exception e)
                    {
                    }
                }

                if (order.GetType().Name != "CompletedOrder")
                {
                    helper.DisplayUserMessage(string.Format("Order not completed... something went wrong :/"), LookupData.MessageList, Brushes.DarkMagenta);
                    return 0;
                }
                else
                {
                    helper.DisplayUserMessage(string.Format("Limit SELL order executed on {0} id = {1}", market, order.OrderUuid), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Market Price: {0}", price), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Total Order Price: {0}", order.Price), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Unit Price: {0}", order.PricePerUnit), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity: {0}", order.Quantity), LookupData.MessageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity remaining: {0}", order.QuantityRemaining), LookupData.MessageList, Brushes.Green);

                    decimal spread = 100 - Math.Abs((Math.Round((Decimal.Divide((order.Price - price), price) * 100), 4)));

                    helper.DisplayUserMessage(string.Format("Spread: {0}%", spread), LookupData.MessageList, Brushes.White);
                    helper.DisplayUserMessage(string.Format("Commission: {0}", order.Commission), LookupData.MessageList, Brushes.White);

                    //update price, to return real price per unit
                    price = order.PricePerUnit;

                    string profit = lastBuyPrice == 0 ? "---" : Math.Round(((Decimal.Divide(price, lastBuyPrice) - 1) * 100), 4).ToString();

                    helper.DisplayUserMessage(string.Format("PROFIT: {0}", profit), LookupData.MessageList, Brushes.Green);

                    totalProfit += Convert.ToDecimal(profit, CultureInfo.InvariantCulture);
                    totalFees += order.Commission;

                }
            }
            catch (Exception ex)
            {
                helper.DisplayUserMessage(string.Format("Error on Sell Order: [0]", ex.Message), LookupData.MessageList, Brushes.Red);
                helper.DisplayUserMessage(ex.InnerException.ToString(), LookupData.MessageList, Brushes.Red);
                helper.DisplayUserMessage(ex.StackTrace.ToString(), LookupData.MessageList, Brushes.Red);
            }
            return price;
        }
        

        public decimal GetBalance(string symbol)
        {
            AccountBalance accountBalance = exchange.GetBalance(symbol);
            return accountBalance.Available;
        }

        public decimal GetCurrentPrice(string baseCurrency, string market)
        {
            JObject ticker = exchange.GetTicker(market);
            return (decimal)ticker["Last"];
        }

    }
}
