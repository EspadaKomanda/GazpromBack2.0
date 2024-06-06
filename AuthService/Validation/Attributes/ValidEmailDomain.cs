using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace BackGazprom.Validation.Attributes;

[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
sealed public class ValidEmailDomain : ValidationAttribute
{
    private static readonly string[] _domains = (Environment.GetEnvironmentVariable("EMAIL_DOMAINS_WHITELIST")??"").Split(",");
    private string? _errorMessage;

    public override bool IsValid(object? value)
    {
        // Ommitting if EMAIL_DOMAINS_WHITELIST is not set
        if (_domains.Length == 0)
        {
            return true;
        }

        try
        {
            if (value != null)
            {
                string email = value.ToString() ?? "";
                string domain = email.Split('@')[1];
                if (!_domains.Contains(domain))
                {
                    _errorMessage = "Email domain is not in the whitelist";
                    return false;
                }
            }
            return true;
        }
        catch
        {
            _errorMessage = "Invalid email address";
            return false;
        }
    }

    public override string FormatErrorMessage(string name)
    {
        return _errorMessage ?? "Email invalidated";
    }

}