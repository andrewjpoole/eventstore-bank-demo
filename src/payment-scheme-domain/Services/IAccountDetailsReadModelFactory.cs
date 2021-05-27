using System.Threading;
using System.Threading.Tasks;

namespace payment_scheme_domain.Services
{
    public interface IAccountDetailsReadModelFactory
    {
        public Task<IAccountDetailsReadModel> Create(int sortCode, int accountNumber, CancellationToken cancellationToken);
    }
}