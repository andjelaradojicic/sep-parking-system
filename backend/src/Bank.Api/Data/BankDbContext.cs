using Microsoft.EntityFrameworkCore;

namespace Bank.Api.Data;

public class BankDbContext(DbContextOptions<BankDbContext> options) : DbContext(options);
