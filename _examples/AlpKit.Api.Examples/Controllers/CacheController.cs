using AlpKit.Common.Constants;
using AlpKit.Logging.Loggers;
using Microsoft.AspNetCore.Mvc;

namespace AlpKit.Api.Examples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CacheController(LoggerServiceBase logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> ExceptionTest()
    {

        var ex = new Exception("exception loglaması yapılacak");
        //Eğer web projesi ise RedirectUrlDataName zorunludur.
        ex.Data[ExceptionDataConstans.RedirectUrlDataName] = "/";
        ex.Data[ExceptionDataConstans.LogTypeDataName] = LogType.Error;
        ex.Data[ExceptionDataConstans.TypeDataName] = "BusinessException";
        ex.Data[ExceptionDataConstans.StatusCodeDataName] = 500;
        throw ex;
    }
    
    public async Task<IActionResult> LogText()
    {
        logger.Info("Loglama Testi");
        return Ok("Loglama Testi");
    }
}
    