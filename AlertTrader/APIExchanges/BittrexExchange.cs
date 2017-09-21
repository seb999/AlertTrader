using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public class BittrexExchange : IAPIExchange
    {
        public BittrexExchange()
        { }

        public void CloseAllpositions()
        {
            throw new NotImplementedException();
        }

        public void GetBalance(string symbol)
        {
            throw new NotImplementedException();
        }

        public decimal GetCurrentPrice()
        {
            throw new NotImplementedException();
        }

        public void InitializeExchange()
        {

        }

        public bool Long()
        { return false; }

        public bool Short()
        { return false; }

    }
}
