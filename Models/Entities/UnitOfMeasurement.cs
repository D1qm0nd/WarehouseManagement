using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

[Table("UnitsOfMeasurement")]
public class UnitOfMeasurement : IConditional
{
    [Key]
    public String Id { get; set; }

    [JsonIgnore]
    public List<Resource> Resource { get; set; }
    public Condition Condition { get; set; }
}