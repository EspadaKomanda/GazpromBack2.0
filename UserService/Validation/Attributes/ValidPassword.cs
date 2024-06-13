using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using UserService.Utils;

namespace UserService.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed public class ValidPassword : ValidationAttribute
{
    private const string Pattern = @"^.{8,256}$";
    private string? _errorMessage;

    public override bool IsValid(object? value)
    {
        try
        {
            if (value != null)
            {
                string password = value.ToString() ?? "";

                if (!Regex.IsMatch(password, Pattern))
                {
                    _errorMessage = "Password must be between 8 and 256 characters";
                    return false;
                }

                if (RockyouChecker.IsInRockyou(password))
                {
                    _errorMessage = "This password is compromised, please choose a different one.";
                    return false;
                }
            }
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            _errorMessage = "Invalid password. Contact administrator if you think this is a mistake.";
            return false;
            throw;
        }
    }

    public override string FormatErrorMessage(string name)
    {
        return _errorMessage ?? "Invalid password";
    }

}