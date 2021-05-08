using System.Text.Json.Serialization;

namespace events.Accounts
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