namespace PaymentReadModel;

public interface IInboundPaymentReadModelFactory
{
    public Task<IInboundPaymentReadModel> Create(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken);
}