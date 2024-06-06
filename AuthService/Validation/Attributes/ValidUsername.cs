using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BackGazprom.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed public class ValidUsername : ValidationAttribute
{
    private const string Pattern = @"^[a-zA-Z0-9_]{3,32}$";

    public override bool IsValid(object? value)
    {
        try
        {
            if (value != null)
            {
                string username = value.ToString() ?? "";

                if (!Regex.IsMatch(username, Pattern))
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
        return "Username must be between 3 and 32 characters and contain only letters, numbers, and underscores";
    }

}