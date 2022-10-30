using LedgerDomain.ReadModel;

namespace LedgerApp.Services;

public class LedgerReadModelFactory : ILedgerReadModelFactory
{
    private readonly IServiceProvider _serviceProvider;

    public LedgerReadModelFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public async Task<ILedgerReadModel> Create(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken)
    {
        if (_serviceProvider.GetService(typeof(ILedgerReadModel)) is not ILedgerReadModel readModel)
            throw new ApplicationException("Couldn't retrieve an instance of InboundPaymentReadModel from the ServiceProvider");

        await readModel.Read(sortCode, accountNumber, cancellationToken);
        return readModel;
    }
}