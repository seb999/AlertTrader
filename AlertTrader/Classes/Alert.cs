using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlertTrader.Classes
{
    public class Alert
    {
        public string orderType;
        public string exchange;
        public int leverage;

        // order = LONG; exchange = KRAKEN; leverage = 2;
        public Alert(string message)
        {
            message.Replace(" ", "");
            string[] parts = message.Split(new char[] { ';' });

            orderType = parts[0].Substring(parts[0].IndexOf("="));
            exchange = parts[1].Substring(parts[1].IndexOf("="));
            leverage = int.Parse(parts[2].Substring(parts[2].IndexOf("=")));

        }
    }
}
