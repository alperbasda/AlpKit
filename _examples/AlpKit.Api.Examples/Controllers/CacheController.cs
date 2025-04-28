using AlpKit.Api.Examples.Models;
using AlpKit.Caching.Helpers;
using Microsoft.AspNetCore.Mvc;

namespace AlpKit.Api.Examples.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class CacheController(ICacheHelper cacheHelper) : ControllerBase
{
    private const string CacheKey = "test";
    [HttpGet]
    public async Task<IActionResult> Read()
    {
        var data = await cacheHelper.GetAsync<CacheModel>(CacheKey);
        if(data == null)
            return NotFound();

        return Ok(data);
    }
    [HttpPost]
    public async Task<IActionResult> Write(CacheModel model)
    {
        // 5 dakika boyunca cache'de kalacak. Hiç okuma yapılazsa 5 dakika sonra cache'den silinecek.
        // Okuma yapılsa bile 2 saat sonra cache'den silinecek.
        await cacheHelper.SetAsync(CacheKey,model,TimeSpan.FromHours(2),TimeSpan.FromMinutes(5));
        return Ok(model);
    }
}
