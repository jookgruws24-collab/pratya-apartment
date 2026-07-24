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

    public DbSet<User> Users => Set<User>();

    public DbSet<UploadedFile> UploadedFiles => Set<UploadedFile>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Tenant ที่ถูกลบ (soft delete) จะไม่ถูกดึงมาแสดงโดยอัตโนมัติ
        modelBuilder.Entity<Tenant>()
            .HasQueryFilter(t => !t.IsDeleted);

        // Username ต้องไม่ซ้ำกัน
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // ใส่ค่าเริ่มต้นของสถานะห้อง/สถานะบิลไว้ในฐานข้อมูลเลย (seed data)
        modelBuilder.Entity<RoomStatus>().HasData(
            new RoomStatus { Id = 1, Name = "Available" },
            new RoomStatus { Id = 2, Name = "Occupied" },
            new RoomStatus { Id = 3, Name = "Maintenance" }
        );

        modelBuilder.Entity<BillStatus>().HasData(
            new BillStatus { Id = 1, Name = "Unpaid" },
            new BillStatus { Id = 2, Name = "Paid" },
            new BillStatus { Id = 3, Name = "Overdue" }
        );
    }
}
