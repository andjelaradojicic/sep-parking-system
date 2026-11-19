using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Psp.Api.Domain;

namespace Psp.Api.Data.Configurations;

public class MerchantPaymentMethodConfiguration : IEntityTypeConfiguration<MerchantPaymentMethod>
{
    public void Configure(EntityTypeBuilder<MerchantPaymentMethod> b)
    {
        b.HasKey(x => x.Id);

        b.HasOne(x => x.Merchant)
            .WithMany(m => m.PaymentMethods)
            .HasForeignKey(x => x.MerchantId)
            .OnDelete(DeleteBehavior.Cascade);

        b.HasOne(x => x.PaymentMethod)
            .WithMany()
            .HasForeignKey(x => x.PaymentMethodId)
            .OnDelete(DeleteBehavior.Restrict);

        b.HasIndex(x => new { x.MerchantId, x.PaymentMethodId }).IsUnique();
    }
}
