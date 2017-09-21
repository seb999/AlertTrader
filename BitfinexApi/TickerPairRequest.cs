using System;
using System.Collections.Generic;
using System.Text;

namespace BitfinexApi
{
    public class TickerPairRequest : PublicRequest
    {
        public TickerPairRequest(string pair)
        {
            this.request = "/v1/pubticker/" + pair;
        }
    
    }
}
