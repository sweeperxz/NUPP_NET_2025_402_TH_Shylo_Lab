namespace University.Infrastructure.Models;

public class CourseModel
{
    public int Id { get; set; }
    public Guid ExternalId { get; set; } = Guid.NewGuid();
    public string? Title { get; set; }
    public int Credits { get; set; }

    // Many-to-one
    public int? InstructorId { get; set; }
    public PersonModel? Instructor { get; set; }

    // Many-to-many
    public ICollection<CourseStudent> CourseStudents { get; set; } = new List<CourseStudent>();
}

public class CourseStudent
{
    public int CourseModelId { get; set; }
    public CourseModel? Course { get; set; }

    public int StudentId { get; set; }
    public PersonModel? Student { get; set; }
}