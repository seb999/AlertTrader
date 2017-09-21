using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;

namespace BitfinexApi
{
    public class GenericRequest
    {
        public string request;
        public string nonce;
        public ArrayList options = new ArrayList() ;
    }

    public class PublicRequest
    {
        public string request;
        public ArrayList options = new ArrayList();
    }
}
