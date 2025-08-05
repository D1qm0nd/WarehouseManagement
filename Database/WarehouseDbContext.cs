using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Database;

public class WarehouseDbContext : DbContext
{
    public DbSet<Client> Clients { get; set; }
    public DbSet<Resource> Resources { get; set; }
    public DbSet<ResourceBalance> ResourceBalances { get; set; }
    public DbSet<UnitOfMeasurement> UnitsOfMeasurement { get; set; }
    public DbSet<ReceiptResource> ResourcesOfReceipt { get; set; }
    public DbSet<ReceiptDocument> ReceiptDocuments { get; set; }
    public DbSet<ShippingResource> ShippingResources { get; set; }
    public DbSet<ShippingDocument> ShippingDocuments { get; set; }


    public WarehouseDbContext()
    {
        
    }
    
    public WarehouseDbContext(DbContextOptionsBuilder<WarehouseDbContext> optionsBuilder) : base(optionsBuilder.Options)
    {
        // this.Database.EnsureCreated();
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options =>
        {
            options.MigrationsHistoryTable("WarehouseMigrations");
        });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("WarehouseDbSchema");
        modelBuilder.UseTablespace("WarehouseDbTableSpace");
        base.OnModelCreating(modelBuilder);
    }
}