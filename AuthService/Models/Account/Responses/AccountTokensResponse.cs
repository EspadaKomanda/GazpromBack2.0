namespace BackGazprom.Models.Account.Responses;

public class AccountTokensResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}