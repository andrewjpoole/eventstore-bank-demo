using System.Text.Json.Serialization;

namespace events.Payments
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentScheme
    {
        Bacs,
        Chaps,
        Fps
    }
}