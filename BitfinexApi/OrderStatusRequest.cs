﻿using System;
using System.Collections.Generic;

using System.Text;

namespace BitfinexApi
{
    public class OrderStatusRequest:GenericRequest
    {
        public long order_id;
        public OrderStatusRequest(string nonce, long order_id)
        {
            this.nonce = nonce;
            this.order_id = order_id;
            this.request = "/v1/order/status";
        }
    }
}
