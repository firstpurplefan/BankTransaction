using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ReadifyBank.Interfaces;
using ReadifyBank.Services;

namespace ReadifyBankTest
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ReadifyBankImpl ingress = new ReadifyBankImpl();
            ingress.OpenHomeLoanAccount("abc", DateTimeOffset.Parse("2016/2/1 22:41:39 +11:00"));
            IAccount a = ingress.AccountList[0];
            ingress.PerformDeposit(a, 50000, "income", DateTimeOffset.Parse("2016/2/2 22:41:39 +11:00"));
            ingress.PerformDeposit(a, 50000, "income", DateTimeOffset.Parse("2016/5/2 22:41:39 +11:00"));
            DateTimeOffset currTime = DateTimeOffset.Now;
            DateTimeOffset toDate = DateTimeOffset.Parse("2016/2/6 22:41:39 +11:00");
            System.Console.WriteLine("The balance of "+a.AccountNumber+" is: ");
            System.Console.WriteLine(ingress.TransactionLog[0].Balance);
            System.Console.WriteLine("The interest of " + a.AccountNumber + " is: ");
            System.Console.WriteLine(ingress.CalculateInterestToDate(a, toDate));
        }
    }
}
