namespace University.Common;

public class Course
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Title { get; set; }
    public int Credits { get; set; }

    
    public Course(string title, int credits)
    {
        Title = title;
        Credits = credits;
    }

    public override string ToString()
    {
        return $"Course: {Title}, Credits: {Credits}";
    }
}