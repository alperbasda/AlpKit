namespace AlpKit.Exceptions.HttpProblemDetails;

public class LogDetail
{
    public string IpAddress { get; set; }
    public string MethodName { get; set; }
    public string User { get; set; }
    public List<LogParameter> Parameters { get; set; }

    public LogDetail()
    {
        IpAddress = string.Empty;
        MethodName = string.Empty;
        User = string.Empty;
        Parameters = new List<LogParameter>();
    }

    public LogDetail(string ipAddress, string methodName, string user, List<LogParameter> parameters)
    {
        IpAddress = ipAddress;
        MethodName = methodName;
        User = user;
        Parameters = parameters;
    }
}
