using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Models.Entities;

namespace Database;

public static class DbContextExtensions
{
    public static void ExecuteSql(this DatabaseFacade facade, List<string> sqlscripts)
    {
        foreach (var script in sqlscripts)
        {
            try
            {
                facade.ExecuteSqlRaw(script);
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}

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
            var schema = this.Model.GetDefaultSchema();
            this.Database.ExecuteSqlRaw(
                $"""
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_RESOURCES()
                    RETURNS TABLE
                            (
                                Id   uuid,
                                Name text
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY SELECT "Id", "Name" FROM "{schema}"."Resources";
                end;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_RECEIPT_RESOURCES()
                    RETURNS TABLE
                            (
                                Id    uuid,
                                Name  text,
                                Count numeric
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                        SELECT RD.Id, RD.Name, SUM(RR."Count")
                        FROM "{schema}".GET_RESOURCES() AS RD
                                 LEFT OUTER JOIN "{schema}"."ReceiptResources" as RR
                                                 ON RD.Id = RR."ResourceId"
                        WHERE RR."Condition" = 0
                        GROUP BY RD.Id, RD.Name;
                
                end;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_SHIPPED_RESOURCES()
                    RETURNS TABLE
                            (
                                Id    uuid,
                                Count numeric
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                        SELECT SR."ResourceId", SUM(SR."Count")
                        FROM "{schema}"."ShippingResources" AS SR
                                 INNER JOIN "{schema}"."ShippingDocument" AS SD
                                            ON SR."ShippingDocumentId" = SD."Id"
                        WHERE "Status" = 2
                        GROUP BY SR."ResourceId";
                end;
                $$ LANGUAGE plpgsql;
                
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_RESOURCES_BALANCES()
                    RETURNS TABLE
                            (
                                Id    uuid,
                                Name  text,
                                Count numeric
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY
                        SELECT RR.Id,
                               RR.Name,
                               COALESCE(CASE
                                            WHEN SR.Count IS NOT NULL
                                                THEN RR.Count - SR.Count
                                            ELSE RR.Count END, 0) AS Count
                        FROM "{schema}".GET_SHIPPED_RESOURCES() AS SR
                                 RIGHT OUTER JOIN "{schema}".GET_RECEIPT_RESOURCES() AS RR
                                                  ON RR.Id = SR.Id;
                end;
                $$ LANGUAGE plpgsql;
                
                CREATE OR REPLACE VIEW "{schema}"."CurrentResourceBalances" AS
                SELECT Id as "Id", Name as "Name", Count as "Count"
                FROM GET_RESOURCES_BALANCES();
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_ACTIVE_CLIENT_SHIPPING_DOCUMENTS(client_id uuid)
                    RETURNS TABLE
                            (
                                "Id"        uuid,
                                "Number"    numeric(20),
                                "Date"      timestamp with time zone,
                                "ClientId"  uuid,
                                "Status"    integer,
                                "Condition" integer
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY SELECT SD."Id", SD."Number", SD."Date", SD."ClientId", SD."Status", SD."Condition"
                                 FROM "{schema}"."ShippingDocument" AS SD
                                 WHERE SD."ClientId" = client_id
                                   AND SD."Condition" = 0;
                END;
                $$ LANGUAGE plpgsql;
                
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION CHECK_ACTIVE_CLIENT_SHIPPING_DOCUMENTS()
                    RETURNS TRIGGER
                AS
                $$
                BEGIN
                    IF (NEW."Condition" = 1) THEN
                        IF EXISTS(SELECT 1 FROM "{schema}".GET_ACTIVE_CLIENT_SHIPPING_DOCUMENTS(OLD."Id"))
                        THEN
                            RAISE EXCEPTION 'Client have active shipping documents records';
                        END IF;
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE TRIGGER ON_ARCHIVE_CLIENT
                    BEFORE UPDATE OF "Condition"
                    ON "{schema}"."Clients"
                    FOR EACH ROW
                EXECUTE FUNCTION "{schema}".CHECK_ACTIVE_CLIENT_SHIPPING_DOCUMENTS();
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_ACTIVE_RESOURCE_SHIPPING(resourceId uuid)
                    RETURNS TABLE
                            (
                                "Id"                  uuid,
                                "ResourceId"          uuid,
                                "ShippingDocumentId"  uuid,
                                "UnitOfMeasurementId" text,
                                "Count"               numeric,
                                "Condition"           integer,
                                "State"               integer
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY SELECT *
                                 FROM "{schema}"."ShippingResources" AS SR
                                 WHERE SR."ResourceId" = resourceId
                                   AND SR."Condition" = 0;
                END
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_ACTIVE_RESOURCE_RECEIPTS(resourceId uuid)
                    RETURNS TABLE
                            (
                                "Id"                  uuid,
                                "ReceiptDocumentId"   uuid,
                                "ResourceId"          uuid,
                                "UnitOfMeasurementId" text,
                                "Count"               numeric,
                                "Condition"           integer
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY SELECT *
                                 FROM "{schema}"."ReceiptResources" AS RR
                                 WHERE RR."ResourceId" = resourceId
                                   AND RR."Condition" = 0;
                END
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION CHECK_ACTIVE_RESOURCE_DOCUMENTS()
                    RETURNS TRIGGER
                AS
                $$
                BEGIN
                    IF (OLD."Condition" = 0 AND NEW."Condition" = 1) THEN
                        IF EXISTS(SELECT 1 FROM "{schema}".GET_ACTIVE_RESOURCE_SHIPPING(OLD."Id")) THEN
                            RAISE EXCEPTION 'Resource is active in shipping';
                        ELSE
                            IF EXISTS(SELECT 1 FROM "{schema}".GET_ACTIVE_RESOURCE_RECEIPTS(OLD."Id")) THEN
                                RAISE EXCEPTION 'Resource is active in receipts';
                            END IF;
                        END IF;
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE TRIGGER ON_ARCHIVE_RESOURCE
                    BEFORE UPDATE
                    ON "{schema}"."Resources"
                    FOR EACH ROW
                EXECUTE FUNCTION "{schema}".CHECK_ACTIVE_RESOURCE_DOCUMENTS();
                
                
                SET search_path TO "WarehouseDbSchema";
                CREATE OR REPLACE FUNCTION GET_ACTIVE_UNIT_OF_MEASUREMENT_RECEIPT_RESOURCES(unitId text)
                    RETURNS TABLE
                            (
                                "Id"                  uuid,
                                "ReceiptDocumentId"   uuid,
                                "ResourceId"          uuid,
                                "UnitOfMeasurementId" text,
                                "Count"               numeric,
                                "Condition"           integer
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY (SELECT *
                                  FROM "{schema}"."ReceiptResources" AS RR
                                  WHERE RR."UnitOfMeasurementId" = unitId AND RR."Condition" = 0);
                END;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_ACTIVE_UNIT_OF_MEASUREMENT_SHIPPING_RESOURCES(unitId text)
                    RETURNS TABLE
                            (
                                "Id"                  uuid,
                                "ResourceId"          uuid,
                                "ShippingDocumentId"  uuid,
                                "UnitOfMeasurementId" text,
                                "Count"               numeric,
                                "Condition"           integer,
                                "State"               integer
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY (SELECT *
                                  FROM "{schema}"."ShippingResources" AS SR
                                  WHERE SR."UnitOfMeasurementId" = unitId AND SR."Condition" = 0);
                END;
                $$ LANGUAGE plpgsql;
                
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION GET_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES(unitId text)
                    RETURNS TABLE
                            (
                                "Id"                  uuid,
                                "Name"                text,
                                "UnitOfMeasurementId" text,
                                "Condition"           integer
                            )
                AS
                $$
                BEGIN
                    RETURN QUERY (SELECT *
                                  FROM "{schema}"."Resources" AS R
                                  WHERE R."UnitOfMeasurementId" = unitId AND R."Condition" = 0);
                END;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE FUNCTION CHECK_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES()
                    RETURNS TRIGGER
                AS
                $$
                BEGIN
                    IF (EXISTS(SELECT 1 FROM "{schema}".GET_ACTIVE_UNIT_OF_MEASUREMENT_SHIPPING_RESOURCES(OLD."Id"))) THEN
                        RAISE EXCEPTION 'Unit of measurement using in shipping resources';
                    ELSE
                        IF (EXISTS(SELECT 1 FROM "{schema}".GET_ACTIVE_UNIT_OF_MEASUREMENT_SHIPPING_RESOURCES(OLD."Id"))) THEN
                            RAISE EXCEPTION 'Unit of measurement using in receipts resources';
                        ELSE
                            IF (EXISTS(SELECT 1 FROM "{schema}".GET_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES(OLD."Id"))) THEN
                                RAISE EXCEPTION 'Unit of measurement using in resources';
                            END IF;
                        END IF;
                    END IF;
                    RETURN NEW;
                END;
                $$ LANGUAGE plpgsql;
                
                SET search_path TO "{schema}";
                CREATE OR REPLACE TRIGGER ON_ARCHIVE_UNIT_OF_MEASUREMENT
                    BEFORE UPDATE
                    ON "{schema}"."UnitsOfMeasurement"
                    FOR EACH ROW
                EXECUTE FUNCTION CHECK_ACTIVE_UNIT_OF_MEASUREMENT_RESOURCES();
                """);
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