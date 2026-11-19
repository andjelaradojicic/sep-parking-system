using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Psp.Api.Auth;
using Psp.Api.Contracts;
using Psp.Api.Services;

namespace Psp.Api.Controllers;

[ApiController]
[Route("api/transactions")]
[Authorize(AuthenticationSchemes = ApiKeyAuthenticationHandler.SchemeName)]
public class TransactionsController(TransactionService transactions, IConfiguration config) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<CreateTransactionResponse>> Create(
        CreateTransactionRequest request, CancellationToken ct)
    {
        var merchantId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var (transaction, created) = await transactions.CreateAsync(merchantId, request, ct);

        var response = new CreateTransactionResponse(
            transaction.Id,
            transaction.Status.ToString().ToUpperInvariant(),
            $"{config["Checkout:BaseUrl"]}/checkout/{transaction.Id}",
            transaction.ExpiresAt);

        return created
            ? Created($"/api/transactions/{transaction.Id}", response)
            : Ok(response);
    }

    [HttpGet("{transactionId:guid}")]
    public async Task<ActionResult<TransactionResponse>> Get(Guid transactionId, CancellationToken ct)
    {
        var merchantId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var t = await transactions.GetAsync(merchantId, transactionId, ct);

        return new TransactionResponse
        {
            TransactionId = t.Id,
            MerchantOrderId = t.MerchantOrderId,
            Status = t.Status.ToString().ToUpperInvariant(),
            Amount = t.Amount,
            Currency = t.Currency,
            PaymentMethod = t.PaymentMethod,
            UpdatedAt = t.UpdatedAt,
        };
    }
}