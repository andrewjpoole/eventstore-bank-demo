﻿namespace SanctionsDomain.RequestHandlers.HeldPayments;

public class ReleaseHeldPaymentResponse
{
    public string PaymentId { get; init; }
    public string ReleasedBy { get; init; }
    public DateTime ReleasedAt { get; init; }
}