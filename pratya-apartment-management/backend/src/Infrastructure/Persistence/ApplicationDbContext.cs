using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(
        DbContextOptions<ApplicationDbContext> options
    ) : base(options)
    {
    }

    public DbSet<Tenant> Tenants => Set<Tenant>();

    public DbSet<Room> Rooms => Set<Room>();

    public DbSet<RoomStatus> RoomStatuses => Set<RoomStatus>();

    public DbSet<BillStatus> BillStatuses => Set<BillStatus>();
    public DbSet<Bill> Bills => Set<Bill>();
}