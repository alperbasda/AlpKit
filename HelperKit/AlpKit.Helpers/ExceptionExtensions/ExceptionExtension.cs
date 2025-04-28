
using AlpKit.Common.Constants;

namespace AlpKit.Helpers.ExceptionExtensions;

public static class ExceptionExtension
{
    public static Exception AddRedirect(this Exception exception, string value = "")
    {
        AddData(exception, ExceptionDataConstans.RedirectUrlDataName, value);
        return exception;
    }
    public static Exception AddLogType(this Exception exception, LogType logType = LogType.Error)
    {
        AddData(exception, ExceptionDataConstans.LogTypeDataName, logType);
        return exception;
    }

    public static Exception AddType(this Exception exception, string exType = "exception")
    {
        AddData(exception, ExceptionDataConstans.TypeDataName, exType);
        return exception;
    }

    public static Exception AddData(this Exception exception, string key, dynamic value)
    {
        exception.Data[key] = value;
        return exception;
    }
}
