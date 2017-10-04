using AlertTrader.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.APIExchanges
{
    public abstract class APIExchange
    {
        protected ObservableCollection<Information> messageList;

        public APIExchange(ObservableCollection<Information> list)
        {
            messageList = list;
        }   

        decimal GetCurrentPrice(string baseCurrency, string market) { return 0; }
        decimal Long() { return 0; }
        decimal Short() { return 0; }
        decimal GetBalance(string symbol) { return 0; }

    }
    //public interface IAPIExchange
    //{
    //    decimal GetCurrentPrice(string baseCurrency, string market);
    //    decimal Long();
    //    decimal Short();
    //    decimal GetBalance(string symbol);
        
    //}

 
}