using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
//using MailKit.Net.Pop3;
using OpenPop.Pop3;
using AlertTrader.Classes;
using OpenPop.Mime;
using System.Text;
using System.Windows.Media;
using System.Media;
using Bittrex;
using System.Linq;
using Newtonsoft.Json.Linq;
using System.Globalization;
using AlertTrader.Misc;
using AlertTrader.APIExchanges;

namespace AlertTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<Budget> budgetList = new ObservableCollection<Budget>();
        ObservableCollection<Information> messageList = new ObservableCollection<Information>();
        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public static  IAPIExchange exchange;
        public static string baseCurrency;
        public static string market;

        public static string email;
        public static string password;

        public static bool usingFixedAmmount;
        public static decimal fixedAmmount;
        public static int limitSpreadPercentage = 1;
        public static int capitalPercentageInEachOrder = 95;
        public static int timerMinutesToCheckEmail = 1;

        public static bool isLong;
        public static decimal ammount;

        public static decimal lastBuyPrice;

        public static decimal totalProfit;
        public static decimal totalFees;


        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            gridBudgetList.ItemsSource = budgetList;
            lbMessageList.ItemsSource = messageList;
            SetTimer();
        }
        
        private void StopTimer_Click(object sender, RoutedEventArgs e)
        {
            dispatcherTimer.Stop();
        }

        private void SetTimer()
        {
            dispatcherTimer.Interval = new TimeSpan(Properties.Settings.Default.timerMinutesToCheckEmail * 60 * 100000); //put 1000 after debugging
            dispatcherTimer.Tick += DispatcherTimer_Tick;
            dispatcherTimer.Start();
        }

        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {
            string message = EmailChecker.CheckEmailAlert();
            helper.DisplayUserMessage(message, messageList, Brushes.Blue);

            StringBuilder builder = new StringBuilder();

            // order = LONG; exchange = KRAKEN; leverage = 2;
            if (message.StartsWith("| "))
            {
                helper.DisplayUserMessage(message, messageList, Brushes.Blue);
            }
            else
            {
                Alert alert = new Alert(message);

                //EXCHANGE MANAGEMENT
                switch(alert.exchange.ToLower())
                {
                    case "kraken":
                        exchange = new KrakenExchange();
                        break;
                    case "bittrex":
                        exchange = new BittrexExchange();
                        break;
                    case "bitfinex":
                        exchange = new BitfinexExchange();
                        break;
                    case "poloniex":
                        exchange = new PoloniexExchange();
                        break;
                    case "1broker":
                        exchange = new OneBrokerExchange();
                        break;
                }


                // ORDER MANAGEMENT
                if (alert.orderType == "LONG" && !isLong)
                {
                    isLong = true;
                    lastBuyPrice = Bu
                }
                else if (alert.orderType == "SHORT" && isLong)
                {
                    decimal sellPrice = Sell();

                    isLong = false;
                    lastBuyPrice = 0;
                }
                else if (alert.orderType == "SHORT" && !isLong)
                {
                    helper.DisplayUserMessage(string.Format("| Found SHORT/SELL before having Long/Buy position opened. Deleting this signal message"), messageList, Brushes.Blue);
                }
                helper.DisplayUserMessage(string.Format("| Deleting signal message"), messageList, Brushes.Blue);

                //----------------------------------------------------------------------

                budgetList.Add(new Budget() { Date = DateTime.Now, TotalProfit = totalProfit, TotalPayedFee = totalFees });
                //after 1 days we purge
                if (budgetList.Count > 60 * 24)
                {
                    budgetList.RemoveAt(0);
                    messageList.RemoveAt(0);
                }
            }
        }
        #region Buy / Sell

        public decimal Buy()
        {
            SystemSounds.Exclamation.Play();

            decimal price = 0;

            try
            {
                Exchange exchange = new Exchange();
                AccountBalance accountBalance = exchange.GetBalance(baseCurrency);
                decimal balance = accountBalance.Available;

                helper.DisplayUserMessage(string.Format("Balance: {0}", balance), messageList, Brushes.White);

                JObject ticker = exchange.GetTicker(market);

                price = (decimal)ticker["Last"];

                helper.DisplayUserMessage(string.Format("Price: {0}", price), messageList, Brushes.White);

                if (usingFixedAmmount)
                {
                    ammount = fixedAmmount;
                }
                else
                {
                    ammount = capitalPercentageInEachOrder * (balance / price);
                }

                decimal limitBuyPrice = price * ((100 + limitSpreadPercentage) / 100);

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
                    helper.DisplayUserMessage(string.Format("Quantity/ammount of {0} not valid. Ammount= {1}", market, ammount), messageList, Brushes.Red);                 
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

        public decimal Sell()
        {
            SystemSounds.Question.Play();

            decimal price = 0;

            try
            {
                Exchange exchange = new Exchange();
                AccountBalance accountBalance = exchange.GetBalance(market);
                decimal balance = accountBalance.Available;

                helper.DisplayUserMessage(string.Format("Balance: {0}", balance), messageList, Brushes.White);

                JObject ticker = exchange.GetTicker(market);

                price = (decimal)ticker["Last"];

                helper.DisplayUserMessage(string.Format("Price: {0}", price), messageList, Brushes.White);

                decimal ammount;
                if (usingFixedAmmount)
                {
                    ammount = fixedAmmount;
                }
                else
                {
                    ammount = capitalPercentageInEachOrder * (balance / price);
                }

                //decimal limitSellPrice = price * ((100 - limitSpreadPercentage) / 100);
                decimal limitSellPrice = price * (decimal.Divide((100 - limitSpreadPercentage), 100));

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

        #endregion


        private void Setting_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
       {
            if (settingTimer == null) return;
            if (settingTimer.Text == "") return;
            Properties.Settings.Default.timerMinutesToCheckEmail = int.Parse(settingTimer.Text);
            Properties.Settings.Default.email = settingEmail.Text;
            Properties.Settings.Default.password = settingPassword.Text;
            Properties.Settings.Default.Save();
        }

    }
}
