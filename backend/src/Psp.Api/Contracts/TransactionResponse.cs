namespace Psp.Api.Contracts;

public record TransactionResponse
{
    public Guid TransactionId { get; init; }
    public string MerchantOrderId { get; init; } = null!;
    public string Status { get; init; } = null!;
    public decimal Amount { get; init; }
    public string Currency { get; init; } = null!;
    public string? PaymentMethod { get; init; }
    public DateTimeOffset UpdatedAt { get; init; }
}