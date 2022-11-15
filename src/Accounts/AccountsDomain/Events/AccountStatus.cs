using System.Text.Json.Serialization;

namespace AccountsDomain.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum AccountStatus
{
    Any,
    Blocked,
    Unblocked,
    Opened,
    Closed
}