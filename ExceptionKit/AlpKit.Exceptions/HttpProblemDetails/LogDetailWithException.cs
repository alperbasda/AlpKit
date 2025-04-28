namespace AlpKit.Exceptions.HttpProblemDetails;

public class LogDetailWithException : LogDetail
{
    public string ExceptionMessage { get; set; }
    public LogDetailWithException()
    {
        ExceptionMessage = string.Empty;
    }

    public LogDetailWithException(
        string ipAddress,
        string methodName,
        string user,
        List<LogParameter> parameters,
        string exceptionMessage) : base(ipAddress, methodName, user, parameters)
    {
        ExceptionMessage = exceptionMessage;
    }
}
