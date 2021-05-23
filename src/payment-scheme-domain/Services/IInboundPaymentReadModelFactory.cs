using System;
using System.Threading.Tasks;

namespace payment_scheme_domain.Services
{
    public interface IInboundPaymentReadModelFactory
    {
        public Task<InboundPaymentReadModel> Create(int sortCode, int accountNumber, Guid correlationId);
    }
}