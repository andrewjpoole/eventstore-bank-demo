using System.Text.Json.Serialization;

namespace PaymentSchemeDomain.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentDirection
{
    Inbound = 1,
    Outbound = 2
}