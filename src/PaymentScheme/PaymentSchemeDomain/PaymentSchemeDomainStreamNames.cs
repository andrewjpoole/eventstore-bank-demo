using PaymentSchemeDomain.Events;

namespace PaymentSchemeDomain
{
    public class PaymentSchemeDomainStreamNames
    {
        public const string AllPayments = "$ce-Payments";
        public static string AccountPayments(PaymentDirection direction, int sortCode, int accountNumber, Guid paymentId) => $"Payments-{Enum.GetName(direction)}-{sortCode}-{accountNumber}-{paymentId}";

        public const string AllInboundPaymentReceived = "$et-InboundPaymentReceived";
        public const string AllInboundPaymentValidated = "$et-InboundPaymentValidated";
        public const string AllInboundPaymentSanctionsChecked = "$et-InboundPaymentSanctionsChecked";
        public const string AllInboundPaymentAccountStatusChecked = "$et-InboundPaymentAccountStatusChecked";
        public const string AllInboundPaymentBalanceUpdated = "$et-InboundPaymentBalanceUpdated";
        public const string AllInboundPaymentHeld = "$et-InboundPaymentHeld";
        public const string AllInboundHeldPaymentReleased = "$et-InboundHeldPaymentReleased";
    }
}