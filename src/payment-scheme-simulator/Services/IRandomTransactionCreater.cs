using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using events.Payments;

namespace payment_scheme_simulator.Services
{
    public interface IRandomInboundPaymentReceivedGenerator
    {
        InboundPaymentReceived_v1 Generate(PaymentScheme scheme, PaymentType type);
    }

    public class RandomInboundPaymentReceivedGenerator : IRandomInboundPaymentReceivedGenerator
    {
        public InboundPaymentReceived_v1 Generate(PaymentScheme scheme, PaymentType type)
        {
            var random = new Random();

            var @event = new InboundPaymentReceived_v1
            {
                Amount = decimal.Parse($"{random.Next(1, 1_000_000)}.{random.Next(0, 99)}"),
                PaymentReference = $"Simulated inbound payment {Guid.NewGuid().ToString().Substring(0, 6)}",
                ProcessingDate = DateTime.Now.Date,
                Scheme = scheme,
                Type = type,
                OriginatingSortCode = 209940,
                OriginatingAccountNumber = random.Next(10000000, 99999999),
                OriginatingAccountName = "",
                DestinationSortCode = 716151,
                DestinationAccountNumber = random.Next(10000000, 99999999),
                DestinationAccountName = ""
            };

            return @event;
        }
    }

    
}
