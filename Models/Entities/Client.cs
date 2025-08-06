using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using Models.Enums;
using Models.Interfaces;

namespace Models.Entities;

[Table("Clients")]
public class Client : IConditional
{
    [Key]
    public Guid Id { get; set; }
    public String Name { get; set; }
    public String Address { get; set; }
    public Condition Condition { get; set; }

    [JsonIgnore]
    public List<ResourceBalance> ResourceBalances { get; set; }
}