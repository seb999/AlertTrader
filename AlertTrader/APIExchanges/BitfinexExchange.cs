using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public class BitfinexExchange : IAPIExchange
    {
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
            throw new NotImplementedException();
        }

        public bool Long()
        {
            throw new NotImplementedException();
        }

        public bool Short()
        {
            throw new NotImplementedException();
        }
    }
}
