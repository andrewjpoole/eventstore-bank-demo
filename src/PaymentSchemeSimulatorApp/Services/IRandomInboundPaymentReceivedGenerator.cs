using System.Threading.Tasks;
using PaymentSchemeDomain.Events;

namespace payment_scheme_simulator.Services;

public interface IRandomInboundPaymentReceivedGenerator
{
    Task<InboundPaymentReceived_v1> Generate(PaymentScheme scheme, PaymentType type, bool simulatedPaymentShouldBeSanctioned = false);
}