namespace University.Common;

public class Teacher : Person
{
    private string Faculty { get; set; }

    public Teacher(string faculty, string name, int age, string email, string phoneNumber, string address)
        : base(name, age, email, phoneNumber, address)
    {
        Faculty = faculty;
    }

    public override string ToString()
    {
        return $"[Teacher] {base.ToString()}, Faculty: {Faculty}";
    }
}