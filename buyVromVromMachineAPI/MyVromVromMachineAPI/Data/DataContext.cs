using Microsoft.EntityFrameworkCore;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=BuyVromVromDb;Trusted_Connection=True;TrustServerCertificate=True;");
    }

    public DbSet<Car> Cars { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Car>().Property(c => c.Price).HasPrecision(18, 2);
        modelBuilder.Entity<User>().HasIndex(u => u.Email).IsUnique();
        modelBuilder.Entity<User>().Property(u => u.Role).HasMaxLength(20).HasDefaultValue("Buyer");
        modelBuilder.Entity<Car>().HasOne<User>().WithMany().HasForeignKey(c => c.SellerId).OnDelete(DeleteBehavior.SetNull);
    }
}
