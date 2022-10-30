using Domain.Interfaces;
using LedgerDomain.Events;
using LedgerDomain.ReadModel;
using LedgerDomain.RequestHandlers;
using Microsoft.Extensions.Logging;

namespace LedgerDomain.Behaviours;

public class PostLedgerEntryBehaviour : IPostLedgerEntryBehaviour
{
    private readonly ILogger<PostLedgerEntryBehaviour> _logger;
    private readonly ILedgerReadModel _ledgerReadModel;
    private readonly IEventPublisher _eventPublisher;

    public PostLedgerEntryBehaviour(ILogger<PostLedgerEntryBehaviour> logger, ILedgerReadModel ledgerReadModel, IEventPublisher eventPublisher)
    {
        _logger = logger;
        _ledgerReadModel = ledgerReadModel;
        _eventPublisher = eventPublisher;
    }

    public async Task<PostLedgerEntryResponse> TryPostLedgerEntry(PostLedgerEntryRequest request)
    {
        await _ledgerReadModel.Read(request.DestinationSortCode, request.DestinationAccountNumber, CancellationToken.None);
        var balance = _ledgerReadModel.CurrentBalance();
        _logger.LogInformation($"ledger for {request.DestinationSortCode} {request.DestinationAccountNumber} read, balance = {balance:C}");

        var transactionId = CreateNewTransactionId();

        if (request.Amount < 0)
        {
            // Debit - check sufficient funds
            if (balance > request.Amount)
            {
                await EmitPostLedgerEntryEvent(request, transactionId);
                return GetPostLedgerEntryResponse(request, transactionId);
            }

            throw new Exception($"insufficient funds in account.");
        }
        
        // Credit
        await EmitPostLedgerEntryEvent(request, transactionId);
        return GetPostLedgerEntryResponse(request, transactionId);

    }

    private async Task EmitPostLedgerEntryEvent(PostLedgerEntryRequest request, Guid transactionId)
    {
        var eventData = new LedgerEntryPosted_v1
        {
            TransactionId = transactionId,
            CorrelationId = request.CorrelationId,
            PaymentId = request.PaymentId,
            Amount = request.Amount,
            Reference = request.Reference,
            DestinationSortCode = request.DestinationSortCode,
            DestinationAccountNumber = request.DestinationAccountNumber,
            OriginatingSortCode = request.OriginatingSortCode,
            OriginatingAccountNumber = request.OriginatingAccountNumber
        };
        await _eventPublisher.Publish(eventData, eventData.StreamName(), CancellationToken.None);
        _logger.LogInformation($"LedgerEntryPosted event sent, S/C:{request.DestinationSortCode} A/C:{request.DestinationAccountNumber} {request.Amount:C}");
    }

    private static PostLedgerEntryResponse GetPostLedgerEntryResponse(PostLedgerEntryRequest request, Guid transactionId) => new(request.DestinationSortCode, request.DestinationAccountNumber, transactionId);

    private static Guid CreateNewTransactionId() => Guid.NewGuid();
}