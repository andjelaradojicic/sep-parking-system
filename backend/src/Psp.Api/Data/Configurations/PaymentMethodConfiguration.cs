using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Psp.Api.Domain;

namespace Psp.Api.Data.Configurations;

public class PaymentMethodConfiguration : IEntityTypeConfiguration<PaymentMethod>
{
    public void Configure(EntityTypeBuilder<PaymentMethod> b)
    {
        b.HasKey(x => x.Id);
        b.Property(x => x.Id).ValueGeneratedNever();
        b.Property(x => x.Code).HasMaxLength(16);
        b.Property(x => x.Name).HasMaxLength(100);
        b.HasIndex(x => x.Code).IsUnique();

        b.HasData(
            new PaymentMethod { Id = 1, Code = "CARD", Name = "Platna kartica", IsAvailable = true },
            new PaymentMethod { Id = 2, Code = "QR", Name = "IPS QR kod", IsAvailable = true }
        );
    }
}
