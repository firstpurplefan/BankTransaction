using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadifyBank.Interfaces;

namespace ReadifyBank.Services
{
    public class AccountImpl : IAccount
    {
        public string AccountNumber
        {
            get; set;        }

        public decimal Balance
        {
            get; set;        }

        public string CustomerName
        {
            get; set;        }

        public DateTimeOffset OpenedDate
        {
            get; set;        }
    }
}
