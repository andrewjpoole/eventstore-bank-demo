﻿namespace LedgerDomain.ReadModel;

public interface ILedgerReadModel
{
    int SortCode { get; }
    int AccountNumber { get; }

    Task Read(int sortCode, int accountNumber, CancellationToken cancellationToken);

    decimal CurrentBalance();
}