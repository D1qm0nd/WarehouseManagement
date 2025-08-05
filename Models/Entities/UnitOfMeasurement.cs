using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

[Table("UnitsOfMeasurement")]
public class UnitOfMeasurement : IConditional
{
    [Key]
    public String Name { get; set; }

    public Condition Condition { get; set; }
}