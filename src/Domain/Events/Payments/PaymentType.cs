using System.Text.Json.Serialization;

namespace Domain.Events.Payments;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentType
{
    Debit,
    Credit,
    RecalledCredit,
    DebitReversal,
    CreditReversal
}