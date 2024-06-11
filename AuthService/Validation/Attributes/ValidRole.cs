using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AuthService.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed public class ValidRole : ValidationAttribute
{
    private const string Pattern = @"^[a-zA-Zа-яА-Я\s0-9_-]{1,16}$";

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
        return "Role name must be between 1 and 16 characters and contain only latin and cyrillic letters, numbers, underscores, hyphens, and spaces";
    }

}