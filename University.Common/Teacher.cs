namespace University.Common;

public class Teacher : Person
{
    public int TeacherId { get; set; } 
    private string Faculty { get; set; }
    public int ExperienceYears { get; set; }
    public string Specialization { get; set; }

    public Teacher(int teacherId,string faculty, string name, int age, string email, string phoneNumber, string address, int experienceYears, string specialization)
        : base(name, age, email, phoneNumber, address)
    {
        TeacherId = teacherId;
        Faculty = faculty;
        ExperienceYears = experienceYears;
        Specialization = specialization;
    }

    public override string ToString()
    {
        return $"[Teacher] {base.ToString()}, Faculty: {Faculty}, Experience: {ExperienceYears} years, Specialization: {Specialization}";
    }

    public static Teacher CreateNew()
    {
        var random = new Random();
        var TeacherId = random.Next(0, 1000000);
        var faculties = new[] { "Mathematics", "Physics", "Computer Science", "Biology", "Chemistry" };
        var specializations = new[] { "Algebra", "Quantum Mechanics", "AI Development", "Genetics", "Organic Chemistry" };

        var faculty = faculties[random.Next(faculties.Length)];
        var specialization = specializations[random.Next(specializations.Length)];
        var name = $"Teacher_{random.Next(1000, 99999)}";
        var age = random.Next(25, 65);
        var email = $"{name.ToLower()}@university.edu";
        var phoneNumber = $"+1-555-{random.Next(1000, 99999):D4}";
        var address = $"{random.Next(100, 9999)} Main St, City {random.Next(1, 100)}";
        var experienceYears = random.Next(1, 40);

        return new Teacher(TeacherId,faculty, name, age, email, phoneNumber, address, experienceYears, specialization);
    }
}