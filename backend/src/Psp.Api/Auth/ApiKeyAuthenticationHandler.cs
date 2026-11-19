using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Psp.Api.Data;

namespace Psp.Api.Auth;

public class ApiKeyAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    PspDbContext db)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    public const string SchemeName = "ApiKey";

    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var merchantId = Request.Headers["X-Merchant-Id"].ToString();

        var apiKey = Request.Headers["X-Api-Key"].ToString();

        if (merchantId.Length == 0 || apiKey.Length == 0)
            return AuthenticateResult.NoResult();

        var merchant = await db.Merchants.AsNoTracking()
            .FirstOrDefaultAsync(m => m.MerchantId == merchantId, Context.RequestAborted);

        if (merchant is null || !merchant.IsActive)
            return AuthenticateResult.Fail("Invalid merchant credentials.");

        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(apiKey));
        if (!CryptographicOperations.FixedTimeEquals(hash, Convert.FromHexString(merchant.ApiKeyHash)))
            return AuthenticateResult.Fail("Invalid merchant credentials.");

        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.NameIdentifier, merchant.Id.ToString())],
            SchemeName);

        return AuthenticateResult.Success(new AuthenticationTicket(new ClaimsPrincipal(identity), SchemeName));
    }
}