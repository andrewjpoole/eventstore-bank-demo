using System.Threading;
using System.Threading.Tasks;

namespace PaymentSchemeApp.Services;

public interface IAccountDetailsReadModelFactory
{
    public Task<IAccountDetailsReadModel> Create(int sortCode, int accountNumber, CancellationToken cancellationToken);
}