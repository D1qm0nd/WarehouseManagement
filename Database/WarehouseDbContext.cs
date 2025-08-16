using System.Reflection;
using Database;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace Database;

public class WarehouseDbContext : DbContext
{
    private static bool _SqlScriptInitialized = false;

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

    // private String GetDirectoryName(String path, int skipChildDirectories = 0)
    // {
    //     var currentDirectory = Path.GetDirectoryName(path);
    //     if (skipChildDirectories > 1)
    //         currentDirectory = GetDirectoryName(currentDirectory, skipChildDirectories-1);
    //     return currentDirectory;
    // }

    public WarehouseDbContext(DbContextOptionsBuilder<WarehouseDbContext> optionsBuilder) : base(optionsBuilder.Options)
    {
        this.Database.EnsureCreated();
        if (_SqlScriptInitialized == false)
        {
            this.RunScripts();
            _SqlScriptInitialized = true;
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options => { options.MigrationsHistoryTable("WarehouseMigrations"); });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("WarehouseDbSchema");
        modelBuilder.UseTablespace("WarehouseDbTableSpace");
        modelBuilder.Entity<ResourceBalance>().ToView("CurrentResourceBalances");
        base.OnModelCreating(modelBuilder);
    }
}