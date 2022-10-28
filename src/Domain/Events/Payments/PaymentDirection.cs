using System.Text.Json.Serialization;

namespace Domain.Events.Payments;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentDirection
{
    Inbound = 1,
    Outbound = 2
}