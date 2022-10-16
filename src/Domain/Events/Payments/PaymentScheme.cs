using System.Text.Json.Serialization;

namespace Domain.Events.Payments;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentScheme
{
    Bacs,
    Chaps,
    Fps
}