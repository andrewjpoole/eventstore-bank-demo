using System;

namespace Domain;

public class StreamNames
{
    public static string SubscriptionGroupName(string streamName) => $"{streamName}-SubscriptionGroup"; // Todo place a number 10,20,30 in this name so the subscriptions are ordered in the eventstore UI?

    /* Stream Naming Strategy
     * ======================
     * Accounts-{sortCode}-{accountNumber}
     * Payments-{inbound/outbound}-{sortCode}-{accountNumber}-{datetime}-{paymentId}
     * Ledger-{accountId}-{datetime}
     * SanctionedNames
     */

    //public class Accounts
    //{
    //    public static string AccountDetails(int sortCode, int accountNumber) => $"Accounts-{sortCode}-{accountNumber}-Details";

    //    public const string AllAccountsOpened = "$et-AccountOpenedEvent";
    //    public const string AllAccounts = "$ce-Accounts"; // category for stream's starting with 'Account' Category projection will create category streams for everything in the Stream name up to the first '-'
    //}
    
    //public class Payments
    //{
    //    public const string AllPayments = "$ce-Payments";
    //    public static string AccountPayments(PaymentDirection direction, int sortCode, int accountNumber, Guid paymentId) => $"Payments-{Enum.GetName(direction)}-{sortCode}-{accountNumber}-{paymentId}";
        
    //    public const string AllInboundPaymentReceived = "$et-InboundPaymentReceived";
    //    public const string AllInboundPaymentValidated = "$et-InboundPaymentValidated";
    //    public const string AllInboundPaymentSanctionsChecked = "$et-InboundPaymentSanctionsChecked";
    //    public const string AllInboundPaymentAccountStatusChecked = "$et-InboundPaymentAccountStatusChecked";
    //    public const string AllInboundPaymentBalanceUpdated = "$et-InboundPaymentBalanceUpdated";
    //    public const string AllInboundPaymentHeld = "$et-InboundPaymentHeld";
    //    public const string AllInboundHeldPaymentReleased = "$et-InboundHeldPaymentReleased";
    //}

    //public class Ledger
    //{
    //    public const string AllLedgerEvents = "$ce-Ledger";
    //    public static string AccountLedger(int sortCode, int accountNumber) => $"Ledger-{sortCode}-{accountNumber}";
    //}

    //public class Sanctions
    //{
    //    public const string GlobalSanctionedNames = "SanctionedNames";
    //    public const string AllSanctionedTransactions = "$projections-SanctionedPayments-result";
    //}
}