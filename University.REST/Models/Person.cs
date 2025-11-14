namespace University.REST.Models;

/// <summary>
/// Модель особи для відповідей API
/// </summary>
public class Person
{
    public Guid Id { get; set; } 
    public Guid ExternalId { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Модель для створення нової особи
/// </summary>
public class PersonCreateModel
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
}

/// <summary>
/// Модель для оновлення особи
/// </summary>
public class PersonUpdateModel
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
}

