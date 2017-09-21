using System;
using System.Collections.Generic;

using System.Text;
using Newtonsoft.Json;
using System.Globalization;

namespace BitfinexApi
{
    public class BalanceResponseItem
    {
        public string type;
        public string currency;
        public string amount;
        public string available;
    }
    public class BalancesResponse
    {
        public Balance trading;
        public Balance deposit;
        public Balance exchange;
        public static Dictionary<string, decimal> balances;

        public static BalancesResponse FromJSON(string response)
        {
            balances = new Dictionary<string, decimal>();
            List<BalanceResponseItem> items = JsonConvert.DeserializeObject<List<BalanceResponseItem>>(response);
            return new BalancesResponse(items);
        }
        private BalancesResponse(List<BalanceResponseItem> items)
        {
            trading = new Balance();
            deposit = new Balance();
            exchange = new Balance();

            Balance cur = new Balance();
            decimal tmp;
            foreach (BalanceResponseItem item in items)
            {

                switch (item.type)
                {
                    case "trading":
                        cur = trading;
                        break;
                    case "deposit":
                        cur = deposit;
                        break;
                    case "exchange":
                        cur = exchange;
                        break;
                }

                //===============
                tmp = decimal.Parse(item.available, CultureInfo.InvariantCulture);
                cur.balances.Add(item.currency, tmp);


                ////switch (item.currency)
                ////{
                ////    case "usd":
                ////        tmp = decimal.Parse(item.available, CultureInfo.InvariantCulture);
                ////        cur.availableUSD = tmp;
                ////        totalAvailableUSD += tmp;
                ////        tmp = decimal.Parse(item.amount, CultureInfo.InvariantCulture);
                ////        cur.USD = tmp;
                ////        totalUSD += tmp;
                ////        break;
                ////    case "btc":
                ////        tmp = decimal.Parse(item.available, CultureInfo.InvariantCulture);
                ////        cur.availableBTC = tmp;
                ////        totalAvailableBTC += tmp;
                ////        tmp=decimal.Parse(item.amount, CultureInfo.InvariantCulture);
                ////        cur.BTC = tmp;
                ////        totalBTC += tmp;
                ////        break;
                ////    case "eth":
                ////        tmp = decimal.Parse(item.available, CultureInfo.InvariantCulture);
                ////        cur.availableETH = tmp;
                ////        totalAvailableETH += tmp;
                ////        tmp = decimal.Parse(item.amount, CultureInfo.InvariantCulture);
                ////        cur.ETH = tmp;
                ////        totalETH += tmp;
                ////        break;
                ////}
            }
        }
    }
    public class Balance
    {
        public Dictionary<string, decimal> balances = new Dictionary<string, decimal>();

        //public decimal USD = 0;
        //public decimal BTC = 0;
        //public decimal ETH = 0;
        //public decimal availableUSD = 0;
        //public decimal availableBTC = 0;
        //public decimal availableETH = 0;
    }
}
