using System;
using System.Threading;
using System.Threading.Tasks;
using events.Accounts;

namespace payment_scheme_domain.Services
{
    public interface IAccountDetailsReadModel
    {
        public int SortCode { get; }
        public int AccountNumber { get; }
        public string Name { get; }
        public AccountStatus Status { get; }
        public DateTime Opened { get; }

        Task Read(int sortCode, int accountNumber, CancellationToken cancellationToken);
    }
}