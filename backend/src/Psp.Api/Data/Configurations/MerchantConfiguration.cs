using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Psp.Api.Domain;

namespace Psp.Api.Data.Configurations;

public class MerchantConfiguration : IEntityTypeConfiguration<Merchant>
{
    public void Configure(EntityTypeBuilder<Merchant> b)
    {
        b.HasKey(x => x.Id);

        b.Property(x => x.MerchantId).HasMaxLength(64);
        b.Property(x => x.Name).HasMaxLength(200);
        b.Property(x => x.Email).HasMaxLength(254);
        b.Property(x => x.ApiKeyHash).HasMaxLength(64);
        b.Property(x => x.SuccessUrl).HasMaxLength(500);
        b.Property(x => x.FailedUrl).HasMaxLength(500);
        b.Property(x => x.ErrorUrl).HasMaxLength(500);

        b.HasIndex(x => x.MerchantId).IsUnique();
        b.HasIndex(x => x.Email).IsUnique();
    }
}
