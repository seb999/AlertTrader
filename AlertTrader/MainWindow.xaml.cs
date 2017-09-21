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
                    lastBuyPrice = exchange.Long();
                }
                else if (alert.orderType == "SHORT" && isLong)
                {
                    decimal sellPrice = exchange.Short();

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
