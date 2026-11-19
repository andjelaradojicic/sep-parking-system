using Sep.Shared.Errors;

namespace Psp.Api.Domain;

public class PspTransaction
{
    public Guid Id { get; set; }

    public Guid MerchantId { get; set; }
    public Merchant Merchant { get; set; } = null!;

    public required string MerchantOrderId { get; set; }
    public DateTimeOffset MerchantTimestamp { get; set; }

    public decimal Amount { get; set; }
    public required string Currency { get; set; }
    public string? Description { get; set; }

    public PspTransactionStatus Status { get; set; }
    public string? PaymentMethod { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset UpdatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }

    private static readonly Dictionary<PspTransactionStatus, PspTransactionStatus[]> AllowedTransitions = new()
    {
        [PspTransactionStatus.Created] = [PspTransactionStatus.Pending, PspTransactionStatus.Expired],
        [PspTransactionStatus.Pending] =
            [
                PspTransactionStatus.Succeeded,
                PspTransactionStatus.Failed,
                PspTransactionStatus.Error,
                PspTransactionStatus.Expired,
            ],
    };

    public bool IsFinal => !AllowedTransitions.ContainsKey(Status);

    public void TransitionTo(PspTransactionStatus next, DateTimeOffset now)
    {
        if (!AllowedTransitions.TryGetValue(Status, out var allowed) || !allowed.Contains(next))
            throw new DomainException(409, "INVALID_STATE",
                $"Transaction cannot transition from {Status} to {next}.");

        Status = next;
        UpdatedAt = now;
    }
}