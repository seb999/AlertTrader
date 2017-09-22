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

namespace AlertTrader.APIExchanges
{
    public class BittrexExchange : IAPIExchange
    {
        Exchange exchange;
        ObservableCollection<Information> messageList = new ObservableCollection<Information>();
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
                if (Properties.Settings.Default.UsingFixedAmmount)
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
                    helper.DisplayUserMessage(string.Format("Order not completed... something went wrong :/"), messageList, Brushes.DarkMagenta);
                    return 0;
                }
                else
                {
                    helper.DisplayUserMessage(string.Format("Limit BUY order executed on {0} id = {1}", market, order.OrderUuid), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Market Price: {0}", price), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Total Order Price: {0}", order.Price), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Unit Price: {0}", order.PricePerUnit), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity: {0}", order.Quantity), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity remaining: {0}", order.QuantityRemaining), messageList, Brushes.Green);

                    decimal spread = 100 - Math.Abs((Math.Round((Decimal.Divide((order.Price - price), price) * 100), 4)));

                    helper.DisplayUserMessage(string.Format("Spread: {0}%", spread), messageList, Brushes.White);
                    helper.DisplayUserMessage(string.Format("Commission: {0}", order.Commission), messageList, Brushes.White);

                    //update price, to return real price per unit
                    price = order.PricePerUnit;
                    totalFees += order.Commission;
                }

            }
            catch (Exception ex)
            {
                helper.DisplayUserMessage(string.Format("Error on Buy Order:"), messageList, Brushes.Red);

                if (ex.Message.Contains("QUANTITY_NOT_PROVIDED"))
                {
                    helper.DisplayUserMessage(string.Format("Quantity/ammount of {0} not valid."), messageList, Brushes.Red);
                }
                else if (ex.Message.Contains("APIKEY_INVALID"))
                {
                    helper.DisplayUserMessage(string.Format("API / KEY INVALID"), messageList, Brushes.Red);
                }
                else
                {
                    helper.DisplayUserMessage(string.Format("ex.Message"), messageList, Brushes.Red);
                    helper.DisplayUserMessage(ex.InnerException.ToString(), messageList, Brushes.Red);
                    helper.DisplayUserMessage(ex.StackTrace.ToString(), messageList, Brushes.Red);
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

                helper.DisplayUserMessage(string.Format("Balance: {0}", balance), messageList, Brushes.White);

                JObject ticker = exchange.GetTicker(market);

                price = (decimal)ticker["Last"];

                helper.DisplayUserMessage(string.Format("Price: {0}", price), messageList, Brushes.White);

                decimal ammount;
                if (Properties.Settings.Default.UsingFixedAmmount)
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
                    helper.DisplayUserMessage(string.Format("Order not completed... something went wrong :/"), messageList, Brushes.DarkMagenta);
                    return 0;
                }
                else
                {
                    helper.DisplayUserMessage(string.Format("Limit SELL order executed on {0} id = {1}", market, order.OrderUuid), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Market Price: {0}", price), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Total Order Price: {0}", order.Price), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Unit Price: {0}", order.PricePerUnit), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity: {0}", order.Quantity), messageList, Brushes.Green);
                    helper.DisplayUserMessage(string.Format("Quantity remaining: {0}", order.QuantityRemaining), messageList, Brushes.Green);

                    decimal spread = 100 - Math.Abs((Math.Round((Decimal.Divide((order.Price - price), price) * 100), 4)));

                    helper.DisplayUserMessage(string.Format("Spread: {0}%", spread), messageList, Brushes.White);
                    helper.DisplayUserMessage(string.Format("Commission: {0}", order.Commission), messageList, Brushes.White);

                    //update price, to return real price per unit
                    price = order.PricePerUnit;

                    string profit = lastBuyPrice == 0 ? "---" : Math.Round(((Decimal.Divide(price, lastBuyPrice) - 1) * 100), 4).ToString();

                    helper.DisplayUserMessage(string.Format("PROFIT: {0}", profit), messageList, Brushes.Green);

                    totalProfit += Convert.ToDecimal(profit, CultureInfo.InvariantCulture);
                    totalFees += order.Commission;

                }
            }
            catch (Exception ex)
            {
                helper.DisplayUserMessage(string.Format("Error on Sell Order: [0]", ex.Message), messageList, Brushes.Red);
                helper.DisplayUserMessage(ex.InnerException.ToString(), messageList, Brushes.Red);
                helper.DisplayUserMessage(ex.StackTrace.ToString(), messageList, Brushes.Red);
            }
            return price;
        }
        

        public decimal GetBalance(string symbol)
        {
            AccountBalance accountBalance = exchange.GetBalance(symbol);
            return accountBalance.Available;
        }

        public decimal GetCurrentPrice(string symbol)
        {
            JObject ticker = exchange.GetTicker(symbol);
            return (decimal)ticker["Last"];
        }

    }
}
