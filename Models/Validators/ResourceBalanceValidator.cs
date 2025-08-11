using System.ComponentModel.DataAnnotations;

namespace Models.Entities;

public class ResourceBalanceValidator
{
    public static ValidationResult Validate(ResourceBalance resourceBalance)
    {
        if (resourceBalance.Id == resourceBalance.ResourceId)
            return ValidationResult.Success;
        return new ValidationResult("ID must be same Resource ID");
    }
}