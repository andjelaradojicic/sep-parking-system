using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Psp.Api.Domain;

namespace Psp.Api.Data;

public static class DbSeeder
{
    private const string ParkingMerchantId = "MRC-PARKING-01";

    public static async Task SeedAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<PspDbContext>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();

        await db.Database.MigrateAsync();

        if (await db.Merchants.AnyAsync(m => m.MerchantId == ParkingMerchantId))
            return;

        var apiKey = config["Seed:ParkingApiKey"]
            ?? throw new InvalidOperationException("Seed:ParkingApiKey in .env not found.");

        var merchant = new Merchant
        {
            MerchantId = ParkingMerchantId,
            Name = "Parking Novi Sad",
            Email = "parking@sep.rs",
            ApiKeyHash = Convert.ToHexStringLower(SHA256.HashData(Encoding.UTF8.GetBytes(apiKey))),
            SuccessUrl = "https://localhost:4201/payment/success",
            FailedUrl = "https://localhost:4201/payment/failed",
            ErrorUrl = "https://localhost:4201/payment/error",
            IsActive = true,
            CreatedAt = DateTimeOffset.UtcNow,
        };
        merchant.PaymentMethods.Add(new MerchantPaymentMethod { PaymentMethodId = 1, IsActive = true });
        merchant.PaymentMethods.Add(new MerchantPaymentMethod { PaymentMethodId = 2, IsActive = true });

        db.Merchants.Add(merchant);
        await db.SaveChangesAsync();
    }
}
