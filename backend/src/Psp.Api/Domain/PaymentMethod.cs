namespace Psp.Api.Domain;

public class PaymentMethod
{
    public int Id { get; set; }
    public required string Code { get; set; }
    public required string Name { get; set; }
    public bool IsAvailable { get; set; }
}
