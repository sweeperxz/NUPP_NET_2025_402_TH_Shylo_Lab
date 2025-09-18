namespace University.Common;

public class Person
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; }
    public int Age { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Address { get; set; }
    

    protected Person(string name, int age, string email, string phoneNumber, string address)
    {
        Name = name;
        Age = age;
        Email = email;
        PhoneNumber = phoneNumber;
        Address = address;
    }

    public override string ToString()
    {
        return $"Name: {Name}, Age: {Age}, Email: {Email}, Phone: {PhoneNumber}, Address: {Address}";
    }
}