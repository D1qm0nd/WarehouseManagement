using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

[Table("Resources")]
public class Resource : IConditional
{
    [Key]
    public Guid Id { get; set; }
    public String Name { get; set; }

    public String UnitOfMeasurementId { get; set; }
    
    [JsonIgnore]
    public UnitOfMeasurement UnitOfMeasurement { get; set; }
    public Condition Condition { get; set; }
}