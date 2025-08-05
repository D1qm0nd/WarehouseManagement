using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Models.Enums;
using Models.Interfaces;
using Models.Validators;

namespace Models.Entities;

[Table("ShippingDocument")]
public class ShippingDocument : IDocument, IConditional
{
    [Key]
    public Guid Id { get; set; }
    public ulong Number { get; set; }
    [CustomValidation(typeof(DateValidator), "IsValid")]
    public DateTime Date { get; set; }
    public Guid ClientId { get; set; }
    public Condition Condition { get; set; }
}