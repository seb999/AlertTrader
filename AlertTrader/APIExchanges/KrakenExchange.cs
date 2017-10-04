using System;
using System.Collections.Generic;
using System.Threading;
using System.Globalization;
using AlertTrader.IAPIExchanges;

namespace AlertTrader.APIExchanges
{
    public class KrakenExchange : IAPIExchange
    {
        KrakenClient.KrakenClient client;

        public KrakenExchange()
        {
            string ApiKey = Properties.Settings.Default.KrakenApiKey;
            string ApiSecret = Properties.Settings.Default.KrakenApiSecret;
            client = new KrakenClient.KrakenClient();
        }
        
        public decimal GetBalance(string symbol)
        {
            var balance = client.GetBalance();
            return decimal.Parse(balance["X" + symbol].ToString());

        }

        public decimal GetCurrentPrice(string baseCurrency, string market)
        {
            var ticker = client.GetTicker(new List<String> { "X" + market + "Z" + baseCurrency });

            return decimal.Parse(ticker["price"].ToString());
        }

        public decimal Long()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            string baseCurrency = Properties.Settings.Default.BaseCurrency;
            string market = Properties.Settings.Default.Market;

            decimal price = 0;

            try
            {
                decimal balance = this.GetBalance(baseCurrency);
                //-------------------------------
                string pair = ("X" + market + "Z" + baseCurrency).ToLower();

                price = this.GetCurrentPrice(baseCurrency, market);

                //Console.WriteLine("Price: " + price);

                decimal ammount;
                if (Properties.Settings.Default.UsingFixedAmount)
                {
                    ammount = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammount = Properties.Settings.Default.CapitalPercentageInEachOrder * (balance / price);
                }

                decimal limitBuyPrice = price * (decimal.Divide((100 + Properties.Settings.Default.LimitSpreadPercentage), 100));

                //------------------------------------------------------
                string decimalString = ammount.ToString();
                if (decimalString.Contains(","))
                {
                    decimalString = decimalString.Replace(",", ".");
                    ammount = Convert.ToDecimal(decimalString);
                }
                //------------------------------------------------------
                var activePositions = client.GetOpenPositions();
                foreach (var pos in activePositions)
                {
                    decimal posAmmount = decimal.Parse(pos.Value.ToString());
                    if (pos.Value.ToString() == pair && posAmmount > 0)
                    {
                        ammount += posAmmount;
                    }
                }
                string lev = @"1:" + Properties.Settings.Default.Leverage;
                var order = client.AddOrder(pair, "sell", "market", ammount, limitBuyPrice, null, lev, "", "", "", "", "", true, null);

                string orderId = order["id"].ToString();

                var closedOrders = client.GetClosedOrders();


                if (!closedOrders.Contains(orderId))
                {

                    //   Console.ForegroundColor = ConsoleColor.Magenta;
                    //   Console.WriteLine("Order was canceled...");
                }
                else
                {
                    //   Console.ForegroundColor = ConsoleColor.Green;
                    //   Console.WriteLine("-----------------------------------------------------------------------------------");
                    //   Console.WriteLine("Market SHORT order executed on " + sellResponse.symbol + " id = " + orderId);
                    //   Console.WriteLine("Price: " + sellStatus.price);
                    //   Console.WriteLine("Average Execution Price: " + sellStatus.avg_execution_price);
                    //   Console.WriteLine("Executed ammount: " + sellStatus.executed_amount);
                    //   Console.WriteLine("Remaining ammount : " + sellStatus.remaining_amount);

                    //update price, to return real price per unit
                    //price = Convert.ToDecimal(sellStatus.avg_execution_price);

                    //  Console.ForegroundColor = ConsoleColor.White;

                    //-------------------------------------------------------------
                    //    logger.Log("------" + DateTime.Now.ToShortDateString() + "--------------------------------------------------------------------");
                    //    logger.Log("Market SHORT order executed on " + sellResponse.symbol + " id = " + orderId);
                    //    logger.Log("Price: " + sellStatus.price);
                    //    logger.Log("Average Execution Price: " + sellStatus.avg_execution_price);
                    //   logger.Log("Executed ammount: " + sellStatus.executed_amount);
                    //    logger.Log("Remaining ammount : " + sellStatus.remaining_amount);

                }
                return price;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error on Short Order: " + ex.Message);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
            }
            return price;
        }


        public decimal Short()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-US");
            string baseCurrency = Properties.Settings.Default.BaseCurrency;
            string market = Properties.Settings.Default.Market;

            decimal price = 0;

            try
            {
                decimal balance = this.GetBalance(baseCurrency);
                //-------------------------------
                string pair = ("X" + market + "Z" +  baseCurrency).ToLower();

                price = this.GetCurrentPrice(baseCurrency, market);

                //Console.WriteLine("Price: " + price);

                decimal ammount;
                if (Properties.Settings.Default.UsingFixedAmount)
                {
                    ammount = decimal.Parse(Properties.Settings.Default.FixedAmmount.ToString());
                }
                else
                {
                    ammount = Properties.Settings.Default.CapitalPercentageInEachOrder * (balance / price);
                }

                decimal limitSellPrice = price * (decimal.Divide((100 - Properties.Settings.Default.LimitSpreadPercentage), 100));

                //------------------------------------------------------
                string decimalString = ammount.ToString();
                if (decimalString.Contains(","))
                {
                    decimalString = decimalString.Replace(",", ".");
                    ammount = Convert.ToDecimal(decimalString);
                }
                //------------------------------------------------------
                var activePositions = client.GetOpenPositions();
                foreach(var pos in activePositions)
                {
                    decimal posAmmount = decimal.Parse(pos.Value.ToString());
                    if (pos.Value.ToString() == pair && posAmmount > 0)
                    {
                        ammount += posAmmount;
                    }
                }
                string lev = @"1:" + Properties.Settings.Default.Leverage;
                var order = client.AddOrder(pair, "sell", "market", ammount, limitSellPrice, null, lev, "", "", "", "", "", true, null);

                string orderId = order["id"].ToString();

                var closedOrders = client.GetClosedOrders();
                

                if(!closedOrders.Contains(orderId))
                {
                
                    //   Console.ForegroundColor = ConsoleColor.Magenta;
                    //   Console.WriteLine("Order was canceled...");
                }
                else
                {
                    //   Console.ForegroundColor = ConsoleColor.Green;
                    //   Console.WriteLine("-----------------------------------------------------------------------------------");
                    //   Console.WriteLine("Market SHORT order executed on " + sellResponse.symbol + " id = " + orderId);
                    //   Console.WriteLine("Price: " + sellStatus.price);
                    //   Console.WriteLine("Average Execution Price: " + sellStatus.avg_execution_price);
                    //   Console.WriteLine("Executed ammount: " + sellStatus.executed_amount);
                    //   Console.WriteLine("Remaining ammount : " + sellStatus.remaining_amount);

                    //update price, to return real price per unit
                    //price = Convert.ToDecimal(sellStatus.avg_execution_price);

                    //  Console.ForegroundColor = ConsoleColor.White;

                    //-------------------------------------------------------------
                    //    logger.Log("------" + DateTime.Now.ToShortDateString() + "--------------------------------------------------------------------");
                    //    logger.Log("Market SHORT order executed on " + sellResponse.symbol + " id = " + orderId);
                    //    logger.Log("Price: " + sellStatus.price);
                    //    logger.Log("Average Execution Price: " + sellStatus.avg_execution_price);
                    //   logger.Log("Executed ammount: " + sellStatus.executed_amount);
                    //    logger.Log("Remaining ammount : " + sellStatus.remaining_amount);

                }
                return price;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error on Short Order: " + ex.Message);
                Console.WriteLine(ex.InnerException);
                Console.WriteLine(ex.StackTrace);
                Console.ForegroundColor = ConsoleColor.White;
            }
            return price;
        }

    }
}
