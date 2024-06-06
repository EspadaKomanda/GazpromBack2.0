using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BackGazprom.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed public class ValidAbout : ValidationAttribute
{
    private const string Pattern = @"^[a-zA-Zа-яА-Я,.;:!?&#@\s]{1,200}$";

    public override bool IsValid(object? value)
    {
        try
        {
            if (value != null)
            {
                string about = value.ToString() ?? "";

                if (!Regex.IsMatch(about, Pattern))
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
        return "About must be between 1 and 200 characters and contain only letters, numbers, and spaces, punctuation, and symbols";
    }

}