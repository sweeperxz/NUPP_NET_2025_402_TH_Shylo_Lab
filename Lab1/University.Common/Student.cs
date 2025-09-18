namespace University.Common;

public class Student : Person
{
    public int StudentId { get; set; } 
    public List<Course> Courses { get; set; } = new();

    public Student(int studentId, string name, int age, string email, string phoneNumber, string address)
        : base(name, age, email, phoneNumber, address)
    {
        StudentId = studentId;
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