using AlpKit.Api.Examples.Data.Repo.MongoRepos;
using AlpKit.Api.Examples.Models;
using AlpKit.Database.NameValueCollectionHelpers;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;

namespace AlpKit.Api.Examples.Controllers;

//Mssql içinde aynı yapı geçerli olacaktır.

[ApiController]
[Route("[controller]/[action]")]
public class MongoController(IMongoTestDataRepository repo) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> Read(Guid id)
    {
        var data = await repo.GetAsync(x => x.Id == id);
        if (data == null)
            return NotFound();

        return Ok(data);
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        //Bir web projesi olsaydı Request üzerinden name value collection alınacaktı.
        var nvc = new NameValueCollection
        {
            { "Name", "Ali" },
            { "Surname", "Veli" }
        };

        var dynamicFilter = nvc.ToDynamicFilter<MongoData>();

        var pageQuery = nvc.ToPageRequest();

        return Ok(await repo.ListDynamicAsync(dynamicFilter, index: pageQuery.PageIndex, size: pageQuery.PageSize));
    }

    [HttpPost]
    public async Task<IActionResult> Add(MongoData model)
    {
        var data = await repo.AddAsync(model); 
        return Ok(data);
    }
    [HttpPost]
    public async Task<IActionResult> Update(MongoData model)
    {
        var data = await repo.AddAsync(model);
        return Ok(data);
    }
}
