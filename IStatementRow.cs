using System;

namespace ReadifyBank.Interfaces
{
    /// <summary>
    /// Statement Row represents a single row in the bank statement
    /// It represents a single transaction on account
    /// </summary>
    public interface IStatementRow
    {
        /// <summary>
        /// Account on which the transaction is made
        /// </summary>
        IAccount Account { set; get; }

        /// <summary>
        /// Date and time of the transaction
        /// </summary>
        DateTimeOffset Date { set; get; }

        /// <summary>
        /// Amount of the operation
        /// </summary>
        decimal Amount { set; get; }

        /// <summary>
        /// Balance of the account after the transaction
        /// </summary>
        decimal Balance { set; get; }

        /// <summary>
        /// Description of the transaction
        /// </summary>
        string Description { set; get; }


    }
}