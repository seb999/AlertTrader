using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BitfinexApi;

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
            throw new NotImplementedException();
        }

        public decimal GetCurrentPrice(string symbol)
        {
            throw new NotImplementedException();
        }

        public decimal Long()
        {
            throw new NotImplementedException();
        }

        public decimal Short()
        {
            throw new NotImplementedException();
        }
    }
}
