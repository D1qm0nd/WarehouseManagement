using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

[Table("ShippingResources")]
public class ShippingResource : IConditional, IStatelessResource
{
    [Key]
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public Guid ShippingDocumentId { get; set; }
    public String UnitOfMeasurementId { get; set; }
    public decimal Count { get; set; }
    [JsonIgnore]
    public Resource Resource  { get; set; }
    [JsonIgnore]
    public UnitOfMeasurement UnitOfMeasurement { get; set; }
    
    [JsonIgnore]
    public ShippingDocument ShippingDocument { get; set; }
    public Condition Condition { get; set; }

    public ResourceState State { get; set; }
}