using Microsoft.EntityFrameworkCore;

namespace Psp.Api.Data;

public class PspDbContext(DbContextOptions<PspDbContext> options) : DbContext(options);
