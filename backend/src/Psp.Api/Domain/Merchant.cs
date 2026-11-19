namespace Psp.Api.Domain;

public class Merchant
{
    public Guid Id { get; set; }
    public required string MerchantId { get; set; }
    public required string Name { get; set; }
    public required string Email { get; set; }

    public required string ApiKeyHash { get; set; }

    public required string SuccessUrl { get; set; }
    public required string FailedUrl { get; set; }
    public required string ErrorUrl { get; set; }

    public bool IsActive { get; set; }
    public DateTimeOffset CreatedAt { get; set; }

    public List<MerchantPaymentMethod> PaymentMethods { get; set; } = [];
}
