using AlertTrader.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public interface IAPIExchange
    {
        decimal GetCurrentPrice(string baseCurrency, string market);
        decimal Long();
        decimal Short();
        decimal GetBalance(string symbol);
        
    }

 
}