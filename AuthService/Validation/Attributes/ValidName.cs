using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AuthService.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed public class ValidName : ValidationAttribute
{
    private const string Pattern = @"^[a-zA-Zа-яА-Я\s]{1,32}$";

    public override bool IsValid(object? value)
    {
        try
        {
            if (value != null)
            {
                string name = value.ToString() ?? "";

                if (!Regex.IsMatch(name, Pattern))
                {
                    return false;
                }
            }
            return true;
        }
        catch
        {
            return false;
        }
    }

    public override string FormatErrorMessage(string name)
    {
        return "First name and last name must be between 1 and 32 characters and contain only letters and spaces";
    }

}