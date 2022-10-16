using System;

namespace Domain;

public class StreamNames
{
    public static string SubscriptionGroupName(string streamName) => $"{streamName}-SubscriptionGroup";
        
    public class Accounts
    {
        public static string AccountDetails(int sortCode, int accountNumber) => $"Account-{sortCode}-{accountNumber}-Details";

        public const string AllAccountsOpened = "$et-AccountOpenedEvent_v1";
        public const string AllAccounts = "$ce-Account"; // category for stream's starting with 'Account' Category projection will create category streams for everything in the Stream name up to the first '-'
        
        public static string AccountTransactions(int sortCode, int accountNumber, Guid correlationId) => $"Account-{sortCode}-{accountNumber}-Transactions-{correlationId}";
        public static string AccountTransactionsSanctions(int sortCode, int accountNumber, Guid correlationId) => $"Account-TransactionSanctions-{sortCode}-{accountNumber}-Transactions-{correlationId}";
        public static string AccountBalanceLedger(int sortCode, int accountNumber) => $"Account-{sortCode}-{accountNumber}-BalanceLedger";
    }

    public class PaymentProcessing
    {
        public const string AllInboundPaymentReceived = "$et-InboundPaymentReceived_v1";
        public const string AllInboundPaymentValidated = "$et-InboundPaymentValidated_v1";
        public const string AllInboundPaymentSanctionsChecked = "$et-InboundPaymentSanctionsChecked_v1";
        public const string AllInboundPaymentAccountStatusChecked = "$et-InboundPaymentAccountStatusChecked_v1";
        public const string AllInboundPaymentBalanceUpdated = "$et-InboundPaymentBalanceUpdated_v1";
        public const string AllInboundPaymentHeld = "$et-InboundPaymentHeld_v1";
    }        

    public class Sanctions
    {
        public const string GlobalSanctionedNames = "GlobalSanctionedNames";
        public const string AllSanctionedTransactions = "$projections-SanctionedPayments-result";
    }
}