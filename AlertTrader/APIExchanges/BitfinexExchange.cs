using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitfinexApi;
using System.Threading;
using System.Globalization;

namespace AlertTrader.APIExchanges
{
    public class BitfinexExchange : IAPIExchange
    {
        BitfinexApiV1 api;

        public BitfinexExchange()
        {
            api = new BitfinexApiV1(Properties.Settings.Default.BitfinexApiKey, Properties.Settings.Default.BitfinexApiSecret);
        }

        public decimal GetBalance(string symbol)
        {
            var balanceResponse = api.GetBalances();
            return balanceResponse.trading.balances[symbol.ToLower()];
        }

        public decimal GetCurrentPrice(string symbol)
        {
            TickerPairResponse ticker = api.GetTicker(symbol);
            return ticker.last_price;
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
                string pair = (market + baseCurrency).ToLower();

                price = this.GetCurrentPrice(pair);

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

                //------------------------------------------------------
                string decimalString = ammount.ToString();
                if (decimalString.Contains(","))
                {
                    decimalString = decimalString.Replace(",", ".");
                    ammount = Convert.ToDecimal(decimalString);
                }
                //------------------------------------------------------

                //CancelAllOrdersResponse cancelOrdersResponse = api.CancelAllOrders();

                ActivePositionsResponse activePositionsResponse = api.GetActivePositions();

                foreach (var pos in activePositionsResponse.positions)
                {
                    decimal posAmmount = decimal.Parse(pos.amount);
                    if (pos.symbol == pair && posAmmount < 0)
                    {
                        ammount -= posAmmount;
                    }
                }

                //------------------------------------------------------


                NewOrderResponse buyResponse = api.ExecuteBuyOrder(pair.ToUpper(), ammount, limitBuyPrice, OrderExchange.Bitfinex, OrderType.MarginMarket);

                string orderId = buyResponse.order_id;

                OrderStatusResponse buyStatus = api.GetOrderStatus(Int64.Parse(buyResponse.order_id));

                if (bool.Parse(buyStatus.is_cancelled))
                {
                    //Console.ForegroundColor = ConsoleColor.Magenta;
                    //Console.WriteLine("Order was canceled...");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("-----------------------------------------------------------------------------------");
                    Console.WriteLine("Market LONG order executed on " + buyResponse.symbol + " id = " + orderId);
                    Console.WriteLine("Price: " + buyStatus.price);
                    Console.WriteLine("Average Execution Price: " + buyStatus.avg_execution_price);
                    Console.WriteLine("Executed ammount: " + buyStatus.executed_amount);
                    Console.WriteLine("Remaining ammount : " + buyStatus.remaining_amount);

                    //update price, to return real price per unit
                    price = Convert.ToDecimal(buyStatus.avg_execution_price);

                    Console.ForegroundColor = ConsoleColor.White;

                    //-------------------------------------------------------------
                //    logger.Log("------" + DateTime.Now.ToShortDateString() + "--------------------------------------------------------------------");
                //    logger.Log("Market LONG order executed on " + buyResponse.symbol + " id = " + orderId);
                //    logger.Log("Price: " + buyStatus.price);
                //    logger.Log("Average Execution Price: " + buyStatus.avg_execution_price);
                //    logger.Log("Executed ammount: " + buyStatus.executed_amount);
                //    logger.Log("Remaining ammount : " + buyStatus.remaining_amount);

                }
                return price;

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Error on Long Order: ");
                Console.WriteLine("ex.Message");
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
                string pair = (market + baseCurrency).ToLower();

                price = this.GetCurrentPrice(pair);

                //Console.WriteLine("Price: " + price);

                decimal ammount;
                if (Properties.Settings.Default.UsingFixedAmmount)
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

                ActivePositionsResponse activePositionsResponse = api.GetActivePositions();

                foreach (var pos in activePositionsResponse.positions)
                {
                    decimal posAmmount = decimal.Parse(pos.amount);
                    if (pos.symbol == pair && posAmmount > 0)
                    {
                        ammount += posAmmount;
                    }
                }

                //------------------------------------------------------
                NewOrderResponse sellResponse = api.ExecuteSellOrder(pair.ToUpper(), ammount, limitSellPrice, OrderExchange.Bitfinex, OrderType.MarginMarket);

                long orderId = long.Parse(sellResponse.order_id);

                OrderStatusResponse sellStatus = api.GetOrderStatus(orderId);

                if (bool.Parse(sellStatus.is_cancelled))
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
                    price = Convert.ToDecimal(sellStatus.avg_execution_price);

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
