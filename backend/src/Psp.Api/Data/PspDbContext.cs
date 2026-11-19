using Microsoft.EntityFrameworkCore;
using Psp.Api.Domain;

namespace Psp.Api.Data;

public class PspDbContext(DbContextOptions<PspDbContext> options) : DbContext(options)
{
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Merchant> Merchants => Set<Merchant>();
    public DbSet<MerchantPaymentMethod> MerchantPaymentMethods => Set<MerchantPaymentMethod>();

    protected override void OnModelCreating(ModelBuilder modelBuilder) =>
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(PspDbContext).Assembly);
}
