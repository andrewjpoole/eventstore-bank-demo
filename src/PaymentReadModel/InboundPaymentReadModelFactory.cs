namespace PaymentReadModel;

public class InboundPaymentReadModelFactory : IInboundPaymentReadModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public InboundPaymentReadModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<IInboundPaymentReadModel> Create(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken)
    {
        if (!(_serviceProvider.GetService(typeof(IInboundPaymentReadModel)) is IInboundPaymentReadModel inboundPaymentReadModel))
            throw new ApplicationException("Couldn't retrieve an instance of InboundPaymentReadModel from the ServiceProvider");

        await inboundPaymentReadModel.Read(sortCode, accountNumber, correlationId, cancellationToken);
        return inboundPaymentReadModel;
    }
}