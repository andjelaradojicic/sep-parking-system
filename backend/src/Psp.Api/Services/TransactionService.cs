using Microsoft.EntityFrameworkCore;
using Npgsql;
using Psp.Api.Contracts;
using Psp.Api.Data;
using Psp.Api.Domain;
using Sep.Shared.Errors;

namespace Psp.Api.Services;

public class TransactionService(PspDbContext db)
{
    private static readonly string[] SupportedCurrencies = ["RSD", "EUR"];
    private static readonly TimeSpan Lifetime = TimeSpan.FromMinutes(15);
    private static readonly TimeSpan MaxTimestampAge = TimeSpan.FromMinutes(5);
    private static readonly TimeSpan MaxTimestampAhead = TimeSpan.FromMinutes(1);


    public async Task<(PspTransaction Transaction, bool Created)> CreateAsync(
        Guid merchantId, CreateTransactionRequest request, CancellationToken ct)
    {
        var now = DateTimeOffset.UtcNow;
        var amount = request.Amount!.Value;
        ValidateAmountAndCurrency(amount, request.Currency);

        var existing = await FindAsync(merchantId, request.MerchantOrderId, ct);
        if (existing is not null)
            return (EnsureSameOrder(existing, amount, request.Currency), false);

        var merchantTimestamp = request.MerchantTimestamp!.Value.ToUniversalTime();
        if (merchantTimestamp < now - MaxTimestampAge || merchantTimestamp > now + MaxTimestampAhead)
            throw new DomainException(400, "VALIDATION_FAILED", "Request validation failed.",
            new Dictionary<string, string[]>
            {
                ["merchantTimestamp"] = ["Must be within 5 minutes in the past and 1 minute in the future."],
            });

        var hasActiveMethod = await db.MerchantPaymentMethods.AnyAsync(
            m => m.MerchantId == merchantId && m.IsActive && m.PaymentMethod.IsAvailable, ct);
        if (!hasActiveMethod)
            throw new DomainException(409, "METHOD_NOT_ACTIVE", "Merchant has no active payment method.");

        var transaction = new PspTransaction
        {
            MerchantId = merchantId,
            MerchantOrderId = request.MerchantOrderId,
            MerchantTimestamp = merchantTimestamp,
            Amount = amount,
            Currency = request.Currency,
            Description = request.Description,
            Status = PspTransactionStatus.Created,
            CreatedAt = now,
            UpdatedAt = now,
            ExpiresAt = now + Lifetime,
        };
        db.Transactions.Add(transaction);

        try
        {
            await db.SaveChangesAsync(ct);
            return (transaction, true);
        }
        catch (DbUpdateException e) when (e.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
        {
            db.Entry(transaction).State = EntityState.Detached;
            var winner = await FindAsync(merchantId, request.MerchantOrderId, ct)
                ?? throw new InvalidOperationException("Unique violation without an existing transaction.");
            return (EnsureSameOrder(winner, amount, request.Currency), false);
        }
    }

    private Task<PspTransaction?> FindAsync(Guid merchantId, string merchantOrderId, CancellationToken ct) =>
        db.Transactions.FirstOrDefaultAsync(
            t => t.MerchantId == merchantId && t.MerchantOrderId == merchantOrderId, ct);

    private static PspTransaction EnsureSameOrder(PspTransaction existing, decimal amount, string currency)
    {
        if (existing.Amount != amount || existing.Currency != currency)
            throw new DomainException(409, "DUPLICATE_ORDER",
                "A transaction with this merchantOrderId already exists with a different amount or currency.");
        return existing;
    }

    private static void ValidateAmountAndCurrency(decimal amount, string currency)
    {
        var errors = new Dictionary<string, string[]>();
        if (amount <= 0 || decimal.Round(amount, 2) != amount)
            errors["amount"] = ["Must be greater than 0 with at most 2 decimal places."];
        if (!SupportedCurrencies.Contains(currency))
            errors["currency"] = ["Supported currencies are RSD and EUR."];

        if (errors.Count > 0)
            throw new DomainException(400, "VALIDATION_FAILED", "Request validation failed.", errors);
    }

    public async Task<PspTransaction> GetAsync(Guid merchantId, Guid transactionId, CancellationToken ct)
    {
       return await db.Transactions.AsNoTracking().FirstOrDefaultAsync(
            t => t.Id == transactionId && t.MerchantId == merchantId, ct)
        ?? throw new DomainException(404, "NOT_FOUND", "Transaction not found.");
    }
}