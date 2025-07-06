namespace AlpKit.Common.Constants;

public class ExceptionDataConstans
{
    public const string RedirectUrlDataName = "RedirectUrl";
    public const string LogTypeDataName = "LogType";
    public const string TypeDataName = "ExceptionType";
    public const string StatusCodeDataName = "StatusCode";

    /* Exception Types */

    public const string NotAuthorizedTypeDataName = "NotAuthorized";
    public const string InvalidAccessToken = "InvalidAccessToken";

    /* Exception Types */
}

public class AppConstants
{
    public const string DefaultLanguage = "tr-TR";
    public const string Unknown = "unknown";
}

public enum LogType
{
    None,
    Info,
    Warn,
    Error
}
