using AlertTrader.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static AlertTrader.Classes.LookupData;

namespace AlertTrader.IAPIExchanges
{
    //public abstract class APIExchange
    //{
    //    protected ObservableCollection<Information> messageList;

    //    public APIExchange(ObservableCollection<Information> list)
    //    {
    //        messageList = list;
    //    }

    //    public decimal GetCurrentPrice(string baseCurrency, string market) { return 0; }
    //    public decimal Long() { return 0; }
    //    public decimal Short() { return 0; }
    //    public decimal GetBalance(string symbol) { return 0; }

    //}
    public interface IAPIExchange
    {
        decimal GetCurrentPrice(string baseCurrency, string market);
        decimal Long();
        decimal Short();
        decimal GetBalance(string symbol);

    }


}