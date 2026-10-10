using Microsoft.EntityFrameworkCore;
using MyVromVromMachineAPI.models;

namespace MyVromVromMachineAPI.Data;

public class DataContext : DbContext
{
    public DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(@"");
    }

}
