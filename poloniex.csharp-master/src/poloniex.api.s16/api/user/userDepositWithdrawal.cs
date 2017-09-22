﻿using Newtonsoft.Json;
using System.Collections.Generic;

namespace XCT.BaseLib.API.Poloniex.User
{
    public interface IDepositWithdrawal
    {
        IList<Deposit> Deposits { get; }

        IList<Withdrawal> Withdrawals { get; }
    }

    public class DepositWithdrawal : IDepositWithdrawal
    {
        [JsonProperty("deposits")]
        public IList<Deposit> Deposits { get; private set; }

        [JsonProperty("withdrawals")]
        public IList<Withdrawal> Withdrawals { get; private set; }
    }
}