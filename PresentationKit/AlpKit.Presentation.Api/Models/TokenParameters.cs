using AlpKit.Presentation.Api.Constants;

namespace AlpKit.Presentation.Api.Models;

public class TokenParameters
{
    public Guid UserId { get; set; } = Guid.Empty;

    public string UserName { get; set; } = default!;

    public string MailAddress { get; set; } = default!;

    public string IpAddress { get; set; } = default!;

    public string UserLanguage { get; set; } = "TR";

    public string AccessToken { get; set; } = default!;

    public string[] Scopes { get; set; } = default!;

    public bool IsSuperUser { get; set; } = false;

    public Dictionary<string, string> Data { get; set; } = [];
}

