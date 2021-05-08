using System.Text.Json.Serialization;

namespace accounts_api.RequestHandlers.Accounts
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum AccountStatus
    {
        Any,
        Blocked,
        Unblocked,
        Opened,
        Closed
    }
}