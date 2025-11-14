using Microsoft.AspNetCore.Mvc;
using University.Infrastructure;
using University.Infrastructure.Models;
using University.REST.Models;

namespace University.REST.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CourseController : ControllerBase
{
    private readonly ICrudServiceAsync<CourseModel> _courseService;

    public CourseController(ICrudServiceAsync<CourseModel> courseService)
    {
        _courseService = courseService;
    }

    /// <summary>
    /// Отримати курс за ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetCourse(Guid id)
    {
        var course = await _courseService.ReadAsync(id);
        if (course == null)
            return NotFound(new { message = "Курс не знайдено" });
        
        var response = MapCourseModelToResponse(course);
        return Ok(response);
    }

    /// <summary>
    /// Отримати всі курси з пагінацією
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAllCourses([FromQuery] int page = 1, [FromQuery] int amount = 10)
    {
        if (page < 1 || amount < 1)
            return BadRequest(new { message = "page та amount повинні бути >= 1" });

        var courses = await _courseService.ReadAllAsync(page, amount);
        var response = courses.Select(MapCourseModelToResponse);
        return Ok(response);
    }

    /// <summary>
    /// Створити новий курс
    /// </summary>
    [HttpPost]
    public async Task<IActionResult> CreateCourse([FromBody] CourseCreateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        if (string.IsNullOrWhiteSpace(model.Title))
            return BadRequest(new { message = "Title обов'язковий" });

        if (model.Credits < 0)
            return BadRequest(new { message = "Credits не може бути від'ємним" });

        var course = new CourseModel
        {
            Title = model.Title,
            Credits = model.Credits,
            InstructorId = model.InstructorId,
            ExternalId = Guid.NewGuid()
        };

        var result = await _courseService.CreateAsync(course);
        if (!result)
            return StatusCode(500, new { message = "Помилка при створенні курсу" });

        await _courseService.SaveAsync();
        var response = MapCourseModelToResponse(course);
        return CreatedAtAction(nameof(GetCourse), new { id = course.ExternalId }, response);
    }

    /// <summary>
    /// Оновити курс за ID
    /// </summary>
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCourse(Guid id, [FromBody] CourseUpdateModel model)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var course = await _courseService.ReadAsync(id);
        if (course == null)
            return NotFound(new { message = "Курс не знайдено" });

        course.Title = model.Title ?? course.Title;
        if (model.Credits.HasValue)
        {
            if (model.Credits < 0)
                return BadRequest(new { message = "Credits не може бути від'ємним" });
            course.Credits = model.Credits.Value;
        }
        if (model.InstructorId.HasValue)
            course.InstructorId = model.InstructorId;

        var result = await _courseService.UpdateAsync(course);
        if (!result)
            return StatusCode(500, new { message = "Помилка при оновленні курсу" });

        await _courseService.SaveAsync();
        return NoContent();
    }

    /// <summary>
    /// Видалити курс за ID
    /// </summary>
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCourse(Guid id)
    {
        var course = await _courseService.ReadAsync(id);
        if (course == null)
            return NotFound(new { message = "Курс не знайдено" });

        var result = await _courseService.RemoveAsync(course);
        if (!result)
            return StatusCode(500, new { message = "Помилка при видаленні курсу" });

        await _courseService.SaveAsync();
        return NoContent();
    }

    /// <summary>
    /// Допоміжний метод для маппінгу CourseModel до Response моделі
    /// </summary>
    private Course MapCourseModelToResponse(CourseModel model)
    {
        return new Course
        {
            Id = model.ExternalId,
            ExternalId = model.ExternalId,
            Title = model.Title ?? string.Empty,
            Credits = model.Credits,
            InstructorId = model.InstructorId
        };
    }
}