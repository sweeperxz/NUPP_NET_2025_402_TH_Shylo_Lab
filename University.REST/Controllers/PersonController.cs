using Microsoft.AspNetCore.Mvc;
using University.Infrastructure;
using University.Infrastructure.Models;
using University.REST.Models;

namespace University.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PersonController : ControllerBase
{
    private readonly ICrudServiceAsync<PersonModel> _personService;

    public PersonController(ICrudServiceAsync<PersonModel> personService)
    {
        _personService = personService;
    }

    /// <summary>
    /// Отримати особу за ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPerson(Guid id)
    {
        var person = await _personService.ReadAsync(id);
        if (person == null)
            return NotFound(new { message = "Особа не знайдена" });
        
        var response = MapPersonModelToResponse(person);
        return Ok(response);
    }

    /// <summary>
    /// Отримати всіх осіб з пагінацією
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllPersons([FromQuery] int page = 1, [FromQuery] int amount = 10)
    {
        if (page < 1 || amount < 1)
            return BadRequest(new { message = "page та amount повинні бути >= 1" });

        var persons = await _personService.ReadAllAsync(page, amount);
        var response = persons.Select(MapPersonModelToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Створити нову особу
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreatePerson([FromBody] PersonCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(model.FirstName) || string.IsNullOrWhiteSpace(model.LastName))
            return BadRequest(new { message = "FirstName та LastName обов'язкові" });

        var person = new PersonModel()
        {
            FirstName = model.FirstName,
            LastName = model.LastName,
            ExternalId = Guid.NewGuid()
        };

        var result = await _personService.CreateAsync(person);
        if (!result)
            return StatusCode(500, new { message = "Помилка при створенні особи" });

        await _personService.SaveAsync();
        var response = MapPersonModelToResponse(person);
        return CreatedAtAction(nameof(GetPerson), new { id = person.ExternalId }, response);
    }

    /// <summary>
    /// Оновити особу за ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePerson(Guid id, [FromBody] PersonUpdateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var person = await _personService.ReadAsync(id);
        if (person == null)
            return NotFound(new { message = "Особа не знайдена" });

        person.FirstName = model.FirstName ?? person.FirstName;
        person.LastName = model.LastName ?? person.LastName;

        var result = await _personService.UpdateAsync(person);
        if (!result)
            return StatusCode(500, new { message = "Помилка при оновленні особи" });

        await _personService.SaveAsync();
        return NoContent();
    }

    /// <summary>
    /// Видалити особу за ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePerson(Guid id)
    {
        var person = await _personService.ReadAsync(id);
        if (person == null)
            return NotFound(new { message = "Особа не знайдена" });

        var result = await _personService.RemoveAsync(person);
        if (!result)
            return StatusCode(500, new { message = "Помилка при видаленні особи" });

        await _personService.SaveAsync();
        return NoContent();
    }

    /// <summary>
    /// Допоміжний метод для маппінгу PersonModel до Response моделі
    /// </summary>
    private Person MapPersonModelToResponse(PersonModel model)
    {
        return new Person
        {
            Id = model.ExternalId,
            ExternalId = Guid.NewGuid(),
            FirstName = model.FirstName ?? string.Empty,
            LastName = model.LastName ?? string.Empty
        };
    }
}

