using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace events
{
    public class SubscriptionNames
    {
        public class Accounts
        {
            public static string AccountOpened(int sortCode, int accountNumber) => $"Account-{sortCode}-{accountNumber}-Details";
            public static string AccountTransactions(int sortCode, int accountNumber) => $"Account-{sortCode}-{accountNumber}-Transactions";
            public static string AccountBalanceLedger(int sortCode, int accountNumber) => $"Account-{sortCode}-{accountNumber}-BalanceLedger";
        }

        public class PaymentProcessing
        {
        }
        

        public class Sanctions
        {
            public const string GlobalSanctionedNames = "GlobalSanctionedNames";
        }
    }
}
