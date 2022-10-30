using System;
using System.Collections.Generic;
using System.Linq;
using MediatR;
using OneOf;
using OneOf.Types;

namespace sanctions_api.RequestHandlers.HeldPayments;

public class ReleaseHeldPaymentRequest : IRequest<ReleaseHeldPaymentResponse>
{
    public Guid PaymentId { get; init; }
    public string ReleasedBy { get; init; }
    public string ReleasedReason { get; init; }
    public Guid HeldPaymentReleaseToken { get; init; }

    public OneOf<True, List<string>> IsValid()
    {
        var validationErrors = new List<string>();
        if (string.IsNullOrEmpty(ReleasedBy))
            validationErrors.Add("ReleasedBy is required");

        if (string.IsNullOrEmpty(ReleasedReason))
            validationErrors.Add("ReleasedReason is required");

        return validationErrors.Any() ? validationErrors : new True();
    }
}