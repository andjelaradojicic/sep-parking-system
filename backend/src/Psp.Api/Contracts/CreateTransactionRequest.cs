using System.ComponentModel.DataAnnotations;

namespace Psp.Api.Contracts;

public record CreateTransactionRequest
{
    [Required, StringLength(64, MinimumLength = 1)]
    public string MerchantOrderId { get; init; } = null!;

    [Required]
    public DateTimeOffset? MerchantTimestamp { get; init; }

    [Required]
    public decimal? Amount { get; init; }

    [Required, StringLength(3, MinimumLength = 3)]
    public string Currency { get; init; } = null!;

    [StringLength(140)]
    public string? Description { get; init; }
}
