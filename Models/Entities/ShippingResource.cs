using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

[Table("ShippingResource")]
public class ShippingResource : IConditional
{
    [Key]
    public Guid Id { get; set; }
    public Guid ResourceId { get; set; }
    public Guid UnitOfMeasurementId { get; set; }
    public ulong Count { get; set; }
    
    [JsonIgnore]
    public Resource Resource  { get; set; }
    [JsonIgnore]
    public UnitOfMeasurement UnitOfMeasurement { get; set; }

    public Condition Condition { get; set; }
}