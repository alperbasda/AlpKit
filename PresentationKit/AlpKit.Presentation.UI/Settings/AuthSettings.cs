namespace AlpKit.Presentation.UI.Settings;

public class AuthSettings
{
    public string LoginUrl { get; set; } = default!;

    public string RefreshTokenEndpoint { get; set; } = "/auth/refresh";

    public string Audience { get; set; } = default!;

    public string Issuer { get; set; } = default!;

    public int AccessTokenExpiration { get; set; }

    public int RefreshTokenExpiration { get; set; }

    public string SecurityKey { get; set; } = default!;

    public List<string> ClientDataKeys { get; set; } = [];
}
