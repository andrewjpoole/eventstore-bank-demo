using System.Text.Json.Serialization;

namespace PaymentSchemeDomain.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentScheme
{
    Bacs,
    Chaps,
    Fps
}