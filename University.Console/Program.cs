using University.Common;

namespace University.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var studentService = new CrudService<Student>();
            var courseService = new CrudService<Course>();
            var teacherService = new CrudService<Teacher>();

            var course1 = new Course("Programming", 5);
            var course2 = new Course("Mathematics", 4);
            var course3 = new Course("History", 3);

            courseService.Create(course1);
            courseService.Create(course2);
            courseService.Create(course3);

            var teacher1 = new Teacher("Computer Science", "Ivan Petrov", 45, "ivan.petrov@uni.com", "380501112233",
                "Kyiv");
            var teacher2 = new Teacher("Mathematics", "Olena Shevchenko", 39, "olena.shev@uni.com", "380631234567",
                "Poltava");

            teacherService.Create(teacher1);
            teacherService.Create(teacher2);

            var student1 = new Student(1, "Oleg", 20, "oleg@mail.com", "123456789", "Poltava");
            var student2 = new Student(2, "Katya", 19, "katya@mail.com", "987654321", "Kharkiv");
            var student3 = new Student(3, "Andriy", 21, "andriy@mail.com", "555777999", "Lviv");

            student1.Enroll(course1);
            student1.Enroll(course2);

            student2.Enroll(course2);
            student2.Enroll(course3);

            student3.Enroll(course1);
            student3.Enroll(course3);

            studentService.Create(student1);
            studentService.Create(student2);
            studentService.Create(student3);

            System.Console.WriteLine("=== Teachers ===");
            foreach (var t in teacherService.ReadAll())
                System.Console.WriteLine(t);

            System.Console.WriteLine("\n=== Courses ===");
            foreach (var c in courseService.ReadAll())
                System.Console.WriteLine(c);

            System.Console.WriteLine("\n=== Students ===");
            foreach (var s in studentService.ReadAll())
                System.Console.WriteLine(s);

            student1.Name = "Oleh Updated";
            studentService.Update(student1);

            System.Console.WriteLine("\n=== After Update Student 1 ===");
            System.Console.WriteLine(studentService.Read(student1.Id));

            studentService.Remove(student2);
            System.Console.WriteLine("\n=== After Removing Student 2 ===");
            foreach (var s in studentService.ReadAll())
                System.Console.WriteLine(s);

            studentService.Save("students.json");
            studentService.Load("students.json");

            System.Console.WriteLine("\n=== Loaded Students from File ===");
            foreach (var s in studentService.ReadAll())
                System.Console.WriteLine(s);
        }
    }
}