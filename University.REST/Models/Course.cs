namespace University.REST.Models;

/// <summary>
/// Модель курсу для відповідей API
/// </summary>
public class Course
{
    public Guid Id { get; set; }
    public Guid ExternalId { get; set; }
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int? InstructorId { get; set; }
}

/// <summary>
/// Модель для створення нового курсу
/// </summary>
public class CourseCreateModel
{
    public string Title { get; set; } = string.Empty;
    public int Credits { get; set; }
    public int? InstructorId { get; set; }
}

/// <summary>
/// Модель для оновлення курсу
/// </summary>
public class CourseUpdateModel
{
    public string? Title { get; set; }
    public int? Credits { get; set; }
    public int? InstructorId { get; set; }
}

