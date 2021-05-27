using System;
using System.Threading;
using System.Threading.Tasks;

namespace payment_scheme_domain.Services
{
    public interface IInboundPaymentReadModelFactory
    {
        public Task<IInboundPaymentReadModel> Create(int sortCode, int accountNumber, Guid correlationId, CancellationToken cancellationToken);
    }
}