using Newtonsoft.Json;
using System;

namespace Jojatekok.OneBrokerAPI.JsonObjects
{
    public class AccountInfo
    {
        [JsonProperty("username")]
        public string Username { get; private set; }

        [JsonProperty("balance_btc")]
        public virtual string BalanceInBitcoins { get; private set; }

        [JsonProperty("email")]
        public string Email { get; private set; }

        [JsonProperty("deposits_unconfirmed_btc")]
        public decimal UnconfirmedDepositsInBitcoins { get; private set; }

        [JsonProperty("registered_since")]
        public DateTime TimeRegistered { get; private set; }
    }
}
