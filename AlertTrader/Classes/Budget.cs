using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.Classes
{
    internal class Budget
    {
        public DateTime Date { get; set; }
        public decimal TotalProfit { get; set; }
        public decimal TotalPayedFee { get; set; }
    }
}
