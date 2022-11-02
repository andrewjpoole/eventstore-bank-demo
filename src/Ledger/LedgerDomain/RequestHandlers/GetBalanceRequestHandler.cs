using LedgerDomain.ReadModel;
using MediatR;
using Microsoft.Extensions.Logging;

namespace LedgerDomain.RequestHandlers;

public class GetBalanceRequestHandler : IRequestHandler<GetBalanceRequest, GetBalanceResponse>
{
    private readonly ILogger<GetBalanceRequestHandler> _logger;
    private readonly ILedgerReadModel _ledgerReadModel;

    public GetBalanceRequestHandler(ILogger<GetBalanceRequestHandler> logger, ILedgerReadModel ledgerReadModel)
    {
        _logger = logger;
        _ledgerReadModel = ledgerReadModel;
    }

    public async Task<GetBalanceResponse> Handle(GetBalanceRequest request, CancellationToken cancellationToken)
    {
        await _ledgerReadModel.Read(request.SortCode, request.AccountNumber, cancellationToken);
        var balance = _ledgerReadModel.CurrentBalance();
        _logger.LogInformation($"ledger for {request.SortCode} {request.AccountNumber} read, balance = {balance:C}");
        return new GetBalanceResponse(request.SortCode, request.AccountNumber, balance);
    }
}