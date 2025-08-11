using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
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
    
    private DateTime _date;
    public DateTime Date { get => _date;
        set
        {
            _date = value.ToUniversalTime();
        }
    }
    public Guid ClientId { get; set; }
    
    public DocumentStatus Status { get; set; }
    
    public Condition Condition { get; set; }
    [JsonIgnore]
    public Client Client { get; set; }
}