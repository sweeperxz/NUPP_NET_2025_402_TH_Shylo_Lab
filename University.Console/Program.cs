using University.Common;

var studentService = new CrudService<Student>();
var teacherService = new CrudService<Teacher>();
const int numberOfStudents = 100_000;
const int numberOfTeachers = 100_000;

var lockObject = new object();
var semaphore = new SemaphoreSlim(3);
var autoResetEvent = new AutoResetEvent(true);

var studentTasks = Enumerable.Range(0, numberOfStudents)
    .Select(async _ =>
    {
        await semaphore.WaitAsync();
        try
        {
            var student = Student.CreateNew();

            lock (lockObject)
            {
                studentService.CreateAsync(student);
            }

            autoResetEvent.WaitOne();
            autoResetEvent.Set();

            return student;
        }
        finally
        {
            semaphore.Release();
        }
    })
    .ToArray();

var teacherTasks = Enumerable.Range(0, numberOfTeachers).Select(async _ =>
    {
        await semaphore.WaitAsync();
        try
        {
            var teacher = Teacher.CreateNew();

            lock (lockObject)
            {
                teacherService.CreateAsync(teacher);
            }

            autoResetEvent.WaitOne();
            autoResetEvent.Set();

            return teacher;
        }
        finally
        {
            semaphore.Release();
        }
    })
    .ToArray();

var students = await Task.WhenAll(studentTasks);
var teachers = await Task.WhenAll(teacherTasks);

var studentAges = students.Select(s => s.Age).ToArray();
var teacherAges = teachers.Select(t => t.Age).ToArray();
int[] studentIds = students.Select(s => s.StudentId).ToArray();
var teacherIds = teachers.Select(t => t.TeacherId).ToArray();

Console.WriteLine("\n=== Statistical Analysis ===");
Console.WriteLine($"Number of students created: {students.Length}");
Console.WriteLine($"Number of teachers created: {teachers.Length}");

Console.WriteLine("\nStudent Age Statistics:");
Console.WriteLine($"Minimum Age: {studentAges.Min()}");
Console.WriteLine($"Maximum Age: {studentAges.Max()}");
Console.WriteLine($"Average Age: {studentAges.Average():F2}");

Console.WriteLine("\nTeacher Age Statistics:");
Console.WriteLine($"Minimum Age: {teacherAges.Min()}");
Console.WriteLine($"Maximum Age: {teacherAges.Max()}");
Console.WriteLine($"Average Age: {teacherAges.Average():F2}");

Console.WriteLine("\nStudent ID Statistics:");
Console.WriteLine($"Minimum ID: {studentIds.Min()}");
Console.WriteLine($"Maximum ID: {studentIds.Max()}");
Console.WriteLine($"Average ID: {studentIds.Average():F2}");

Console.WriteLine("\nTeacher ID Statistics:");
Console.WriteLine($"Minimum ID: {teacherIds.Min()}");
Console.WriteLine($"Maximum ID: {teacherIds.Max()}");
Console.WriteLine($"Average ID: {teacherIds.Average():F2}");


await studentService.SaveAsync();