namespace Psp.Api.Contracts;

public record CreateTransactionResponse(
    Guid TransactionId,
    string Status,
    string CheckoutUrl,
    DateTimeOffset ExpiresAt);