using OneOf;
using OneOf.Types;

namespace PaymentSchemeDomain.Validation;

public static class ValidationExtensions
{
    public static OneOf<True, string> IsValidUKSortCode(this int sortcode) =>
        sortcode switch
        {
            > 999999 => "too large for a UK SortCode",
            < 100000 => "too small for a UK SortCode",
            _ => new True()
        };

    public static OneOf<True, string> IsValidUKAccountNumber(this int accountNumber) =>
        accountNumber switch
        {
            > 99999999 => "too large for a UK Account Number",
            < 10000000 => "too small for a UK Account Number",
            _ => new True()
        };

    public static OneOf<True, string> ContainsValidCharacters(this string value)
    {
        var validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-_ .";
        if (value.Any(character => !validChars.Contains(character)))
            return $"Value contains illegal characters, valid characters are {validChars}";

        return new True();
    }

    public static OneOf<True, string> IsValidAccountName(this string value) => value.Length > 50 ? "Account Names have a max length of 50" : value.ContainsValidCharacters();
    public static OneOf<True, string> IsValidReference(this string value) => value.Length > 100 ? "A payment Reference has a max length of 100" : value.ContainsValidCharacters();
    
    public static void UseError(this OneOf<True, string> validationResult, Action<string> useValidationError)
    {
        if (validationResult.IsT1)
            useValidationError(validationResult.AsT1);
    }
}