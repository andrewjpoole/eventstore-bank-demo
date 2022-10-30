namespace LedgerDomain.ReadModel;

public interface ILedgerReadModelFactory
{
    public Task<ILedgerReadModel> Create(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken);
}