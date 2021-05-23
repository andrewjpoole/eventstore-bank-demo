using System.Text.Json.Serialization;

namespace events.Payments
{
    [JsonConverter(typeof(JsonStringEnumConverter))]
    public enum PaymentType
    {
        Debit,
        Credit,
        RecalledCredit,
        DebitReversal,
        CreditReversal
    }
}