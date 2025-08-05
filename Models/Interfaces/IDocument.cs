using System.ComponentModel.DataAnnotations;
using Models.Validators;

namespace Models.Interfaces;

public interface IDocument
{
    public Guid Id { get; set; }
    public ulong Number { get; set; }
    
    [CustomValidation(typeof(DateValidator), "IsValid")]
    public DateTime Date { get; set; }
}