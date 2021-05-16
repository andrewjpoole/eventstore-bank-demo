namespace events
{
    public class StreamNames
    {
        public class Accounts
        {
            public static string AccountOpened(int sortCode, int accountNumber) => $"Account-{sortCode}-{accountNumber}-Details";

            public const string AllAccountsOpened = "$et-AccountOpenedEvent_v1";
            public const string AllAccounts = "$ce-Account";
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
