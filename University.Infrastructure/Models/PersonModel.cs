namespace University.Infrastructure.Models;

public class PersonModel
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; } = Guid.NewGuid();
    public string? FirstName { get; set; }
    public string? LastName { get; set; }

    // One-to-many
    public ICollection<CourseModel> Courses { get; set; } = new List<CourseModel>();

    // One-to-one
    public ProfileModel? Profile { get; set; }
}

public class ProfileModel
{
    public int Id { get; set; }
    public string? Bio { get; set; }
    public DateTime? BirthDate { get; set; }
    
    public int PersonModelId { get; set; }
    public PersonModel? Person { get; set; }
}