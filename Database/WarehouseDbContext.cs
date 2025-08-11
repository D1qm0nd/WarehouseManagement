using System.Reflection;
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
        // var domain = AppDomain.CurrentDomain.BaseDirectory;
        // var dir = GetDirectoryName(domain,5);
        // var path = dir+"/Scripts/script.sql";
        // using (StreamReader sr = new StreamReader(dir))
        // {
            // String script = sr.ReadToEnd();
            // this.Database.ExecuteSqlRaw(script);
        // }
        
        String script = $"""
                        SET search_path TO "{this.Model.GetDefaultSchema()}";
                        CREATE OR REPLACE FUNCTION "ON_RECEIPT_RESOURCES_UPDATE_FUNC"()
                            RETURNS TRIGGER AS
                        $$
                        BEGIN
                            INSERT INTO "{this.Model.GetDefaultSchema()}"."ResourceBalances" ("Id", "ResourceId", "UnitOfMeasurementId", "Count", "Condition")
                            VALUES (OLD."ResourceId", OLD."ResourceId", OLD."UnitOfMeasurementId", OLD."Count", 0);

                        END;
                        $$ LANGUAGE plpgsql;

                        CREATE OR REPLACE TRIGGER "UPDATE_BALANCE_AFTER_UPDATE" AFTER INSERT ON "{this.Model.GetDefaultSchema()}"."ReceiptResources"
                            FOR EACH ROW 
                            EXECUTE FUNCTION "ON_RECEIPT_RESOURCES_UPDATE_FUNC"();
                        """;
        this.Database.ExecuteSqlRaw(script);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseNpgsql(options => { options.MigrationsHistoryTable("WarehouseMigrations"); });
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("WarehouseDbSchema");
        modelBuilder.UseTablespace("WarehouseDbTableSpace");
        base.OnModelCreating(modelBuilder);
    }
}