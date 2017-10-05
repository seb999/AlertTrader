using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Threading;
using AlertTrader.Classes;
using System.Text;
using System.Windows.Media;
using AlertTrader.Misc;
using AlertTrader.APIExchanges;
using static AlertTrader.Classes.LookupData;
using AlertTrader.IAPIExchanges;
using Serilog;

namespace AlertTrader
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        ObservableCollection<Budget> budgetList = new ObservableCollection<Budget>();
        //ObservableCollection<Information> messageList = new ObservableCollection<Information>();
        ObservableCollection<Information> messageList = LookupData.MessageList;

        DispatcherTimer dispatcherTimer = new DispatcherTimer();

        public static IAPIExchange exchange;
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
            Log.Logger = helper.OpenLogger();
            Log.Information("AlertTrader started");
        }

        private void StartTimer_Click(object sender, RoutedEventArgs e)
        {
            gridBudgetList.ItemsSource = budgetList;
            lbMessageList.ItemsSource = LookupData.MessageList;
            SetTimer();

            Log.Information("timer started");
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
            helper.DisplayUserMessage(message, LookupData.MessageList, Brushes.Blue);

            StringBuilder builder = new StringBuilder();

            // order = LONG; exchange = KRAKEN; leverage = 2;
            if (message.StartsWith("| "))
            {
                helper.DisplayUserMessage(message, LookupData.MessageList, Brushes.Blue);
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
                       // exchange = new BittrexExchange();
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
                Log.Warning("TotalProfit : " + totalProfit + " TotalPayedFee : " + totalFees);
                //after 1 days we purge
                if (budgetList.Count > 60 * 24)
                {
                    budgetList.RemoveAt(0);
                    messageList.RemoveAt(0);
                }
            }
        }
        private void Setting_CheckBoxChanged(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.UsingFixedAmount = chkUsingFixedAmount.IsChecked.Value;
            Properties.Settings.Default.Save();
        }

        private void Setting_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
       {
            if (settingTimer == null || settingEmail == null || settingPassword == null || txtBaseCurrency == null
                || txtMarket == null || txtCapitalPercentageInEachOrder == null || chkUsingFixedAmount == null
                || txtSettingFixedAmmount == null || txtLimitSpreadPercentage == null || txtLeverage == null
                || txtBittrexApiKey == null || txtBittrexApiSecret == null || txtBitfinexApiKey == null || txtBitfinexApiSecret == null
                || txtPoloniexApiKey == null || txtPoloniexApiSecret == null || txtKrakenApiKey == null || txtKrakenApiSecret == null
                || txtOneBrokerApiToken == null)
            {
                return;
            }
            if (settingTimer.Text == "" || settingEmail.Text == "" || settingPassword.Text == "" || txtBaseCurrency.Text == ""
                || txtMarket.Text == "" || txtCapitalPercentageInEachOrder.Text == "" || txtSettingFixedAmmount.Text == ""
                || txtLimitSpreadPercentage.Text == "" || txtLeverage.Text == "" || txtBitfinexApiKey.Text == "" || txtBitfinexApiSecret.Text == ""
                || txtBitfinexApiKey.Text == "" || txtBitfinexApiSecret.Text == "" || txtPoloniexApiKey.Text == "" || txtPoloniexApiSecret.Text == ""
                || txtKrakenApiKey.Text == "" || txtKrakenApiSecret.Text == "" || txtOneBrokerApiToken.Text == "")
            {
                return;
            }

            Properties.Settings.Default.timerMinutesToCheckEmail = int.Parse(settingTimer.Text);
            Properties.Settings.Default.email = settingEmail.Text;
            Properties.Settings.Default.password = settingPassword.Text;

            Properties.Settings.Default.BaseCurrency = txtBaseCurrency.Text;
            Properties.Settings.Default.Market = txtMarket.Text;
            Properties.Settings.Default.CapitalPercentageInEachOrder = decimal.Parse(txtCapitalPercentageInEachOrder.Text);
            Properties.Settings.Default.FixedAmmount = txtSettingFixedAmmount.Text;
            Properties.Settings.Default.LimitSpreadPercentage = int.Parse(txtLimitSpreadPercentage.Text);
            Properties.Settings.Default.Leverage = txtLeverage.Text;
            Properties.Settings.Default.BittrexApiKey = txtBittrexApiKey.Text;
            Properties.Settings.Default.BittrexApiSecret = txtBittrexApiSecret.Text;
            Properties.Settings.Default.BitfinexApiKey = txtBitfinexApiKey.Text;
            Properties.Settings.Default.BitfinexApiSecret = txtBitfinexApiSecret.Text;
            Properties.Settings.Default.PoloniexApiKey = txtPoloniexApiKey.Text;
            Properties.Settings.Default.PoloniexApiSecret = txtPoloniexApiSecret.Text;
            Properties.Settings.Default.OneBrokerApiToken = txtOneBrokerApiToken.Text;
            Properties.Settings.Default.KrakenApiKey = txtKrakenApiKey.Text;
            Properties.Settings.Default.KrakenApiSecret = txtKrakenApiSecret.Text;

            Properties.Settings.Default.Save();
        }

    }
}
