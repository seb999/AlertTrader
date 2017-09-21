using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitfinexApi
{
    public class TickerPairResponse
    {
        public decimal mid; //[price] (bid + ask) / 2
        public decimal bid; //[price] Innermost bid
        public decimal ask; //[price] Innermost ask
        public decimal last_price; //[price] The price at which the last order executed
        public decimal low; //[price] Lowest trade price of the last 24 hours
        public decimal high; //[price] Highest trade price of the last 24 hours
        public decimal volume; //[price] Trading volume of the last 24 hours
        public string timestamp; //[time]  The timestamp at which this information was valid

        public static TickerPairResponse FromJSON(string response)
        {
            TickerPair tickerPair = JsonConvert.DeserializeObject<TickerPair>(response);
            return new TickerPairResponse(tickerPair);
        }

        private TickerPairResponse(TickerPair ticketPair)
        {
            this.mid = ticketPair.mid;
            this.bid = ticketPair.bid;
            this.ask = ticketPair.ask;
            this.last_price = ticketPair.last_price;
            this.low = ticketPair.low;
            this.high = ticketPair.high;
            this.volume = ticketPair.volume;
            this.timestamp = ticketPair.timestamp;
        }
        public class TickerPair
        {
            public decimal mid = 0;
            public decimal bid = 0;
            public decimal ask = 0;
            public decimal last_price = 0;
            public decimal low = 0;
            public decimal high = 0;
            public decimal volume = 0;
            public string timestamp = string.Empty;
        }
    }
}