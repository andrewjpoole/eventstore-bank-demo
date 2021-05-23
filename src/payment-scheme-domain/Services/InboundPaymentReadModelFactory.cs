using System;
using System.Threading.Tasks;

namespace payment_scheme_domain.Services
{
    public class InboundPaymentReadModelFactory : IInboundPaymentReadModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public InboundPaymentReadModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<InboundPaymentReadModel> Create(int sortCode, int accountNumber, Guid correlationId)
        {
            var inboundPaymentReadModel = _serviceProvider.GetService(typeof(InboundPaymentReadModel)) as InboundPaymentReadModel;

            if (inboundPaymentReadModel is null)
                throw new ApplicationException(
                    "Couldn't retrieve an instance of InboundPaymentReadModel from the ServiceProvider");

            await inboundPaymentReadModel.Read(sortCode, accountNumber, correlationId);
            return inboundPaymentReadModel;
        }
    }
}