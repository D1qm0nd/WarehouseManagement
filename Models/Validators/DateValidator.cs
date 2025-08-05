using System.ComponentModel.DataAnnotations;
using Models.Exceptions;

namespace Models.Validators;

public class DateValidator
{
    public static ValidationResult IsValid(DateTime date)
    {
        if (date.Kind == DateTimeKind.Utc)
            return ValidationResult.Success;
        return new ValidationResult("Date must be in UTC");
    }
}