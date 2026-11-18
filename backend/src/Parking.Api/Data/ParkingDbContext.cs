using Microsoft.EntityFrameworkCore;

namespace Parking.Api.Data;

public class ParkingDbContext(DbContextOptions<ParkingDbContext> options) : DbContext(options);
