using Domain.Events.Payments;

namespace PaymentReadModel;

public interface IInboundPaymentReadModelFactory
{
    public Task<IInboundPaymentReadModel> Create(PaymentDirection paymentDirection, int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken);
}