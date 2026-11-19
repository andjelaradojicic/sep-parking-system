namespace Psp.Api.Domain;

public class MerchantPaymentMethod
{
    public Guid Id { get; set; }
    public Guid MerchantId { get; set; }
    public Merchant Merchant { get; set; } = null!;
    public int PaymentMethodId { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public bool IsActive { get; set; }
}
