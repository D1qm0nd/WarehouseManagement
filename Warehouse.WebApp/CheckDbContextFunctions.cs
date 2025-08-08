using Database;
using Models.Entities;

namespace Warehouse.WebApp;

public static class CheckDbContextFunctions
{
    public static bool CheckClientExists(this WarehouseDbContext context, Client client) =>
        context.Clients.Any(c => c.Name == client.Name);
    
    public static bool CheckReceiptDocumentWithNumber(this WarehouseDbContext context, ReceiptDocument receiptDocument) =>
    context.ReceiptDocuments.Any(rd => rd.Number == receiptDocument.Number);
    
    public static bool CheckOtherClientWithNameExists(this WarehouseDbContext context, Client client) =>
        context.Clients.Any(c => c.Name == client.Name && c.Id != client.Id);
    
    
    public static bool CheckResourceExists(this WarehouseDbContext context, Resource resource) =>
        context.Clients.Any(c => c.Name == resource.Name);

    public static bool CheckOtherResourceWithNameExists(this WarehouseDbContext context, Resource resource) =>
        context.Resources.Any(r => r.Name == resource.Name && r.Id != resource.Id);
}