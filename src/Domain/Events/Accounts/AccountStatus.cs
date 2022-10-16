using System.Text.Json.Serialization;

namespace Domain.Events.Accounts;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountStatus
{
    Any,
    Blocked,
    Unblocked,
    Opened,
    Closed
}