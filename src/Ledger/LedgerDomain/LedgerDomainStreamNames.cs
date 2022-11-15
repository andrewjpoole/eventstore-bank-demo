namespace LedgerDomain;

public class LedgerDomainStreamNames
{
    public const string AllLedgerEvents = "$ce-Ledger";
    public static string AccountLedger(int sortCode, int accountNumber) => $"Ledger-{sortCode}-{accountNumber}";
}