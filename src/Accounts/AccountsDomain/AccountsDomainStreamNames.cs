namespace AccountsDomain;

public class AccountsDomainStreamNames
{
    public static string AccountDetails(int sortCode, int accountNumber) => $"Accounts-{sortCode}-{accountNumber}-Details";

    public const string AllAccountsOpened = "$et-AccountOpenedEvent";
    public const string AllAccounts = "$ce-Accounts"; // category for stream's starting with 'Account' Category projection will create category streams for everything in the Stream name up to the first '-'
}