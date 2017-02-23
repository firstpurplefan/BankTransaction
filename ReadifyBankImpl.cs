using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadifyBank.Interfaces;

namespace ReadifyBank.Services
{
    public class ReadifyBankImpl : IReadifyBank
    {
        public IList<IAccount> AccountList
        {get; set;
        }

        public IList<IAccount> LnAccList
        {
            get; set;        }
         
        public IList<IAccount> SvAccList
        {
            get; set;        }

        public IList<IStatementRow> TransactionLog
        {
            get; set;        }

        InstanceFactory factory; 

        public ReadifyBankImpl()
        {
            this.factory = new InstanceFactory();
            this.AccountList = new List<IAccount>();
            this.LnAccList = new List<IAccount>();
            this.SvAccList = new List<IAccount>();
            this.TransactionLog = new List<IStatementRow>();
        }
        /// <summary>
        /// Calculate interest rate for an account from now to a specific previous time
        /// The interest rate for Saving account is 6% monthly
        /// The interest rate for Home loan account is 3.99% annually
        /// </summary>
        /// <param name="account">Customer account</param>
        /// <param name="toDate">Calculate interest to this date</param>
        /// <returns>The added value</returns>
        public decimal CalculateInterestToDate(IAccount account, DateTimeOffset toDate)
        {
            DateTimeOffset currTime = DateTimeOffset.Now;
            List<IStatementRow> miniStat = (from log in this.TransactionLog
                                                  where (log.Account.Equals(account) && log.Date > toDate)
                                                  orderby log.Date
                                                  select log).ToList();
            if (miniStat.Count == 0)
            {
                if (account.Balance == 0)
                    return 0;
                else
                {
                    decimal interest = 0;
                    if (account.AccountNumber.Contains("SV"))
                    {
                        interest = account.Balance * (decimal)0.06 *
                            ((decimal)(currTime.ToUnixTimeSeconds() - toDate.ToUnixTimeSeconds()) / 2592000);
                        return interest;
                    }
                    else
                    {
                        interest = account.Balance * (decimal)0.0399 *
                            ((decimal)(currTime.ToUnixTimeSeconds() - toDate.ToUnixTimeSeconds()) / 31536000);
                        return interest;

                    }
                }
            }
            if (account.AccountNumber.Contains("SV"))
            {
                decimal interest = 0;
                for (int i = 0; i < miniStat.Count - 1; i++)
                {
                    interest = interest + miniStat[i].Balance * (decimal)0.06 * 
                        ((decimal)(miniStat[i + 1].Date.ToUnixTimeSeconds() - miniStat[i].Date.ToUnixTimeSeconds()) / 2592000);
                }
                interest = interest + miniStat[miniStat.Count - 1].Balance * (decimal)0.06 * 
                        ((decimal)(currTime.ToUnixTimeSeconds() - miniStat[miniStat.Count - 1].Date.ToUnixTimeSeconds()) / 2592000);
                return interest;
            }
            else
            {
                decimal interest = 0;
                for (int i = 0; i < miniStat.Count - 1; i++)
                {
                    interest = interest + miniStat[i].Balance * (decimal)0.0399 * 
                        ((decimal)(miniStat[i + 1].Date.ToUnixTimeSeconds() - miniStat[i].Date.ToUnixTimeSeconds()) / 31536000);
                }
                interest = interest + miniStat[miniStat.Count - 1].Balance * (decimal)0.0399 * 
                        ((decimal)(currTime.ToUnixTimeSeconds() - miniStat[miniStat.Count - 1].Date.ToUnixTimeSeconds()) / 31536000);
                return interest;
            }
        }

        public IEnumerable<IStatementRow> CloseAccount(IAccount account, DateTimeOffset closeDate)
        {

            var row = (IStatementRow) factory.generateInstance(typeof(IStatementRow));
            row.Account = account;
            row.Date = closeDate;
            row.Amount = 0;
            row.Balance = 0;
            row.Description = "Close Account.";
            this.TransactionLog.Add(row);
            lock (AccountList)
            {
                AccountList.Remove(account);
            }
            lock (LnAccList)
            {
                LnAccList.Remove(account);
            }
            lock (SvAccList)
            {
                SvAccList.Remove(account);
            }
            IEnumerable<IStatementRow> fullStats = from log in this.TransactionLog
                                                   where log.Account.Equals(account)
                                                   select log;
            return fullStats;
        }

        public decimal GetBalance(IAccount account)
        {
            return account.Balance;
        }
        
        public IEnumerable<IStatementRow> GetMiniStatement(IAccount account)
        {
            IEnumerable<IStatementRow> miniStat = (from log in this.TransactionLog
                                                   where log.Account.Equals(account)
                                                   orderby log.Date descending
                                                   select log).Take(5); 
            return miniStat;
        }

        public IAccount OpenHomeLoanAccount(string customerName, DateTimeOffset openDate)
        {
            var NewAcc = (IAccount)factory.generateInstance(typeof(IAccount));
            lock (LnAccList)
            {
                int LnLength = LnAccList.Count;
                int AccNum = LnLength + 1;
                string AccountNumber = String.Format("{0:LN-000000}", AccNum);
                NewAcc.AccountNumber = AccountNumber;
                NewAcc.Balance = 0;
                NewAcc.CustomerName = customerName;
                NewAcc.OpenedDate = openDate;
                //NewAcc = new Account(openDate, customerName, AccountNumber, 0);
                this.AccountList.Add(NewAcc);
                this.LnAccList.Add(NewAcc);
            }
            return NewAcc;
        }

        public IAccount OpenSavingsAccount(string customerName, DateTimeOffset openDate)
        {
            var NewAcc = (IAccount)factory.generateInstance(typeof(IAccount));
            lock (SvAccList)
            {
                int SvLength = this.SvAccList.Count;
                int AccNum = SvLength + 1;
                string AccountNumber = String.Format("{0:SV-000000}", AccNum);
                NewAcc.AccountNumber = AccountNumber;
                NewAcc.Balance = 0;
                NewAcc.CustomerName = customerName;
                NewAcc.OpenedDate = openDate;
                this.AccountList.Add(NewAcc);
                this.SvAccList.Add(NewAcc);
            }
            return NewAcc;
        }

        public void PerformDeposit(IAccount account, decimal amount, string description, DateTimeOffset depositDate)
        {
            var NewStat = (IStatementRow)factory.generateInstance(typeof(IStatementRow));
            lock (account)
            {
                decimal balance = account.Balance + amount;
                account.Balance = balance;
                NewStat.Account = account;
                NewStat.Date = depositDate;
                NewStat.Amount = amount;
                NewStat.Balance = balance;
                NewStat.Description = description;
                this.TransactionLog.Add(NewStat);
            }
        }

        public void PerformTransfer(IAccount from, IAccount to, decimal amount, string description, DateTimeOffset transferDate)
        {
            var NewStat = (IStatementRow)factory.generateInstance(typeof(IStatementRow));
            lock (from)
            {
                decimal balance = from.Balance;
                if (balance < amount)
                {
                    NewStat.Account = from;
                    NewStat.Date = transferDate;
                    NewStat.Amount = amount;
                    NewStat.Balance = balance;
                    NewStat.Description = description + "**Trascation Fail!**";
                    this.TransactionLog.Add(NewStat);
                }
                else
                {
                    from.Balance = from.Balance - amount;
                    to.Balance = to.Balance + amount;
                    NewStat.Account = from;
                    NewStat.Date = transferDate;
                    NewStat.Amount = amount;
                    NewStat.Balance = balance;
                    NewStat.Description = description;
                    this.TransactionLog.Add(NewStat);
                }
            }
        }

        public void PerformWithdrawal(IAccount account, decimal amount, string description, DateTimeOffset withdrawalDate)
        {
            var NewStat = (IStatementRow)factory.generateInstance(typeof(IStatementRow));
            lock (account)
            {
                decimal balance = account.Balance;
                if (balance < amount)
                {
                    NewStat.Account = account;
                    NewStat.Date = withdrawalDate;
                    NewStat.Amount = amount;
                    NewStat.Balance = balance;
                    NewStat.Description = description + "**Trascation Fail!**";
                    this.TransactionLog.Add(NewStat);
                }
                else
                {
                    account.Balance = account.Balance - amount;
                    NewStat.Account = account;
                    NewStat.Date = withdrawalDate;
                    NewStat.Amount = amount;
                    NewStat.Balance = balance;
                    NewStat.Description = description;
                    this.TransactionLog.Add(NewStat);
                }
            }
        }
    }
}
