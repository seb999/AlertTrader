using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public class KrakenExchange : IAPIExchange
    {
        public void CloseAllpositions()
        {
            throw new NotImplementedException();
        }

        public decimal GetBalance(string symbol)
        {
            throw new NotImplementedException();
        }

        public decimal GetCurrentPrice(string symbol)
        {
            throw new NotImplementedException();
        }

        public void InitializeExchange()
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
