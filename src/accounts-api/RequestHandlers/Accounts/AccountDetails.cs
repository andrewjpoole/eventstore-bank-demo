namespace accounts_api.RequestHandlers.Accounts
{
    public class AccountDetails
    {
        public string Id { get; set; }
        public string SortCode { get; init; }
        public string AccountNumber { get; init; }
        public decimal Balance { get; init; }
        public AccountStatus Status { get; set; }
    }
}