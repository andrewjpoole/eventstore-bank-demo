using System.Text.Json.Serialization;

namespace PaymentSchemeDomain.Events;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum PaymentType
{
    Debit,
    Credit,
    RecalledCredit,
    DebitReversal,
    CreditReversal
}