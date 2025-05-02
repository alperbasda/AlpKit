namespace AlpKit.Presentation.Api.Models;

public partial class TokenParameters
{
    public string[]? ClientIds { get; set; }
    
    public Guid UserId { get; set; } = Guid.Empty;

    public string UserName { get; set; } = default!;

    public string MailAddress { get; set; } = default!;

    public string IpAddress { get; set; } = default!;

    public string UserLanguage { get; set; } = "TR";

    public string AccessToken { get; set; } = default!;

    public string Currency { get; set; } = "TRY";

    public string[] Scopes { get; set; } = default!;

    public bool IsSuperUser { get; set; } = false;

    public string DeviceId { get; set; } = "";

    public string DeviceToken { get; set; } = "";

    public string OffsetStr { get; set; } = "00:00:00.00";

    public TimeSpan Offset => TimeSpan.Parse(OffsetStr);

}

