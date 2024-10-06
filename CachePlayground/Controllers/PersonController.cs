using CachePlayground.Models;
using CachePlayground.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace CachePlayground.Controllers;

[ApiController]
[Route("[controller]")]
public class PersonController : ControllerBase
{
    private readonly IPersonService personService;

    public PersonController(IPersonService personService)
    {
        this.personService = personService;
    }

    [HttpGet]
    [ProducesResponseType<Person>(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> GetPerson([FromQuery] string name, [FromQuery] int age, CancellationToken ct)
    {
        var person = await this.personService.GetAsync(name, age, ct);
        if (person is null)
        {
            return this.BadRequest();
        }
        return this.Ok(person);
    }

    [HttpPut]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> PutPerson([FromBody] PersonRequestDto request, CancellationToken ct)
    {
        await this.personService.PutAsync(request, ct);
        return this.NoContent();
    }
}