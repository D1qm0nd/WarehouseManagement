using Database;
using FakerDotNet;
using Microsoft.EntityFrameworkCore;
using Models.Entities;

namespace DataLoadTest;

internal static class ContextOptionsBuilder
{
    public static DbContextOptionsBuilder<WarehouseDbContext> CreateOptions()
    {
        var optionsBuilder = new DbContextOptionsBuilder<WarehouseDbContext>();
        var connectionString = "host=localhost;port=5432;database=WarehouseDb;username=WarehouseDb;password=WarehouseDb";
        optionsBuilder.UseNpgsql(connectionString);
        return optionsBuilder;
    }
}

public class Tests
{
    private WarehouseDbContext _context;
    private readonly int _clientCount = 100;
    private readonly int _resourceCount = 100;
    private readonly int _unitsOfMeasurementCount = 10;
    private readonly Random _random = new Random();
    private int _shippingDocumentsCount = 20;
    
    [OneTimeSetUp]
    public void Setup()
    {
        var options = ContextOptionsBuilder.CreateOptions();
        _context = new WarehouseDbContext(options);
    }

    [OneTimeTearDown]
    public void TearDown()
    {
        _context.Dispose();
    }


    public IEnumerable<Client> GenerateClients()
    {
        for (int i = 0; i < _clientCount; i++)
        {
            yield return new Client()
            {
                Id = Guid.NewGuid(),
                Name = Faker.Name.NameWithMiddle(),
                Address = Faker.Address.StreetAddress(),
            };
        }
    }

    public IEnumerable<Resource> GenerateResources(List<UnitOfMeasurement> unitOfMeasurements)
    {
        for (int i = 0; i < _resourceCount; i++)
        {
            yield return new Resource()
            {
                Id = Guid.NewGuid(),
                Name = Faker.Commerce.ProductName(),
                UnitOfMeasurementId = unitOfMeasurements.ElementAt(_random.Next(0, unitOfMeasurements.Count)).Id,
            };
        }
    }

    public IEnumerable<UnitOfMeasurement> GenerateUnitsOfMeasurements()
    {
        for (int i = 0; i < _unitsOfMeasurementCount; i++)
        {
            yield return new UnitOfMeasurement()
            {
                Id = Faker.Random.Element(["kg", "unit", "meter", "cubic meter"])
            };
        }
    }

    public IEnumerable<ReceiptDocument> GenerateReceiptDocuments()
    {
        for (int i = 0; i < _resourceCount; i++)
        {
            yield return new ReceiptDocument()
            {
                Id = Guid.NewGuid(),
                Number = (ulong)i + 1000,
                Date = Faker.Date.Backward(100),
            };
        }
    }

    public IEnumerable<ReceiptResource> GenerateReceiptResources(IEnumerable<ReceiptDocument> documents,
        List<Resource> resources, List<UnitOfMeasurement> unitOfMeasurements)
    {
        foreach (var document in documents)
        {
            for (var i = 0; i < _random.Next(0, 10); i++)
            {
                yield return new ReceiptResource()
                {
                    Id = Guid.NewGuid(),
                    ReceiptDocumentId = document.Id,
                    ResourceId = resources.ElementAt(_random.Next(0, resources.Count())).Id,
                    UnitOfMeasurementId = unitOfMeasurements.ElementAt(_random.Next(0, unitOfMeasurements.Count())).Id,
                    Count = Decimal.TryParse(_random.NextDouble().ToString(), out decimal count) ? count : 10
                };
            }
        }
    }

    public IEnumerable<ShippingDocument> GenerateShippingDocuments(List<Client> clients)
    {
        for (int i = 0; i < _shippingDocumentsCount; i++)
        {
            yield return new ShippingDocument()
            {
                Id = Guid.NewGuid(),
                Number = (ulong)i + 2000,
                Status = _random.Next(0, 2).ToDocumentStatus(),
                Date = Faker.Date.Backward(10),
                ClientId = clients.ElementAt(_random.Next(0, clients.Count())).Id,
            };
        }
    }

    public IEnumerable<ShippingResource> GenerateShippingResources(List<ShippingDocument> documents,
        IEnumerable<Resource> resources, IEnumerable<UnitOfMeasurement> unitOfMeasurements)
    {
        for (int i = 0; i < _shippingDocumentsCount; i++)
        {
            for (var j = 0; j < _random.Next(0, 10); i++)
            {
                yield return new ShippingResource()
                {
                    Id = Guid.NewGuid(),
                    ResourceId = resources.ElementAt(_random.Next(0, resources.Count())).Id,
                    ShippingDocumentId = documents.ElementAt(_random.Next(0,documents.Count())).Id,
                    UnitOfMeasurementId = unitOfMeasurements.ElementAt(_random.Next(0, unitOfMeasurements.Count())).Id,
                    Count = Decimal.TryParse(_random.NextDouble().ToString(), out decimal count) ? count : 10
                };
            }
        }
    }

    [Test]
    public void Load()
    {
        var units = GenerateUnitsOfMeasurements().DistinctBy(c => c.Id).ToList();
        var resources =  GenerateResources(units).DistinctBy(c => c.Id).DistinctBy(c => c.Name).ToList();
        var receiptsDocuments = GenerateReceiptDocuments().DistinctBy(c => c.Id).ToList();
        var receiptsResources = GenerateReceiptResources(receiptsDocuments, resources, units).DistinctBy(c => c.Id).ToList();
        var clients = GenerateClients().DistinctBy(c => c.Id).ToList();
        var shippingDocuments = GenerateShippingDocuments(clients).DistinctBy(c => c.Id).ToList();
        var shippingResources = GenerateShippingResources(shippingDocuments, resources, units).DistinctBy(c => c.Id).ToList();
        _context.AddRange(units);
        _context.AddRange(resources);
        _context.AddRange(receiptsDocuments);
        _context.AddRange(receiptsResources);
        _context.AddRange(clients);
        _context.AddRange(shippingDocuments);
        _context.AddRange(shippingResources);
        _context.SaveChanges();
    }
}