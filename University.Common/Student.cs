namespace University.Common;

public class Student : Person
{
    private static readonly Random _random = new Random();
    public int StudentId { get; set; } 
    public List<Course> Courses { get; set; } = new();

    public Student(int studentId, string name, int age, string email, string phoneNumber, string address)
        : base(name, age, email, phoneNumber, address)
    {
        StudentId = studentId;
    }

    public static Student CreateNew()
    {
        var studentId = _random.Next(1000, 10000);
        var age = _random.Next(17, 30);
        var name = $"Student_{studentId}";
        var email = $"student{studentId}@university.com";
        var phone = $"38050{_random.Next(1000000, 9999999)}";
        var address = $"Address_{_random.Next(1, 100)}";
        
        return new Student(studentId, name, age, email, phone, address);
    }

    public void Enroll(Course course)
    {
        Courses.Add(course);
    }

    public override string ToString()
    {
        return $"[Student] {base.ToString()}, ID: {StudentId}, Courses: {Courses.Count}";
    }
}