using System;
using System.Threading;
using System.Threading.Tasks;

namespace payment_scheme_domain.Services
{
    public class AccountDetailsReadModelFactory : IAccountDetailsReadModelFactory
    {
        private readonly IServiceProvider _serviceProvider;

        public AccountDetailsReadModelFactory(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<IAccountDetailsReadModel> Create(int sortCode, int accountNumber, CancellationToken cancellationToken)
        {
            if (!(_serviceProvider.GetService(typeof(IAccountDetailsReadModel)) is IAccountDetailsReadModel accountDetailsReadModel))
                throw new ApplicationException(
                    "Couldn't retrieve an instance of AccountDetailsReadModel from the ServiceProvider");

            await accountDetailsReadModel.Read(sortCode, accountNumber, cancellationToken);
            return accountDetailsReadModel;
        }
    }
}