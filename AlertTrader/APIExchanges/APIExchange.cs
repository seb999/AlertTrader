using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public interface IAPIExchange
    {
        decimal GetCurrentPrice();
        bool Long();
        bool Short();
        void CloseAllpositions();

        void GetBalance(string symbol);
    }

 
}