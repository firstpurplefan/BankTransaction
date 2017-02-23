using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadifyBank.Interfaces;

namespace ReadifyBank.Services
{
    /// <summary>
    /// This is an implementation of interface StaementRow
    /// Statement Row represents a single row in the bank statement
    /// It represents a single transaction on account
    /// </summary>
    public class StatementRowImpl : IStatementRow
    {
        public IAccount Account
        {
            get; set;        }

        public decimal Amount
        {
            get; set;        }

        public decimal Balance
        {
            get; set;        }

        public DateTimeOffset Date
        {
            get; set;        }

        public string Description
        {
            get; set;        }

    }
}
