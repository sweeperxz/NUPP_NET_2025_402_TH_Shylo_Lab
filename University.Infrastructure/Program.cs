using University.Infrastructure;
using University.Infrastructure.Models;

var dbFile = Path.Combine(AppContext.BaseDirectory, "university.db");
var backupFile = Path.Combine(AppContext.BaseDirectory, "university_backup.json");

await using (var ctx = new UniversityContext(dbFile))
{
    ctx.Database.EnsureCreated();
}

var personRepo = new EfRepository<PersonModel>(dbFile);
var courseRepo = new EfRepository<CourseModel>(dbFile);

var personService = new CrudServiceAsync<PersonModel>(personRepo, backupFile);
var courseService = new CrudServiceAsync<CourseModel>(courseRepo, backupFile);

var person = new PersonModel { FirstName = "Ivan", LastName = "Petrov" };
await personService.CreateAsync(person);


var profile = new ProfileModel { Bio = "Lecturer in CS", BirthDate = new DateTime(1980, 5, 1), PersonModelId = person.Id };
var profileRepo = new EfRepository<ProfileModel>(dbFile);
await profileRepo.AddAsync(profile);

var course = new CourseModel { Title = "Algorithms", Credits = 5, InstructorId = person.Id };
await courseService.CreateAsync(course);


Console.WriteLine("\nSeeding additional test data...");

var extraPeople = new List<PersonModel>
{
    new() { FirstName = "Maria", LastName = "Ivanova" },
    new() { FirstName = "John", LastName = "Doe" },
    new() { FirstName = "Alice", LastName = "Smith" },
    new() { FirstName = "Bob", LastName = "Johnson" }
};

foreach (var p in extraPeople)
{
    await personService.CreateAsync(p);
}

var additionalProfiles = new List<ProfileModel>
{
    new() { Bio = "PhD student in ML", BirthDate = new DateTime(1996, 3, 12), PersonModelId = extraPeople[0].Id }, 
    new() { Bio = "Undergrad, CS", BirthDate = new DateTime(2002, 7, 4), PersonModelId = extraPeople[2].Id },   
};

foreach (var pf in additionalProfiles)
{
    await profileRepo.AddAsync(pf);
}

var extraCourses = new List<CourseModel>
{
    new() { Title = "Data Structures", Credits = 4, InstructorId = extraPeople[0].Id }, 
    new() { Title = "Databases", Credits = 3, InstructorId = extraPeople[1].Id },       
    new() { Title = "Artificial Intelligence", Credits = 6, InstructorId = person.Id } 
};

foreach (var c in extraCourses)
{
    await courseService.CreateAsync(c);
}

var courseStudentRepo = new EfRepository<CourseStudent>(dbFile);

PersonModel FindPerson(string first, string last)
{
    var all = personService.ReadAllAsync().GetAwaiter().GetResult().ToList();
    return all.FirstOrDefault(x => string.Equals(x.FirstName, first, StringComparison.OrdinalIgnoreCase)
                                   && string.Equals(x.LastName, last, StringComparison.OrdinalIgnoreCase))!;
}

CourseModel FindCourse(string title)
{
    var all = courseService.ReadAllAsync().GetAwaiter().GetResult().ToList();
    return all.FirstOrDefault(x => string.Equals(x.Title, title, StringComparison.OrdinalIgnoreCase))!;
}

var enrollments = new Dictionary<string, List<(string first, string last)>>
{
    ["Algorithms"] = new List<(string, string)> { ("Maria", "Ivanova"), ("Alice", "Smith") },
    ["Data Structures"] = new List<(string, string)> { ("John", "Doe"), ("Bob", "Johnson") },
    ["Databases"] = new List<(string, string)> { ("Alice", "Smith"), ("Ivan", "Petrov") },
    ["Artificial Intelligence"] = new List<(string, string)> { ("Maria", "Ivanova"), ("John", "Doe"), ("Bob", "Johnson") }
};

foreach (var kv in enrollments)
{
    var courseFound = FindCourse(kv.Key);
    if (courseFound == null)
    {
        Console.WriteLine($"Warning: course '{kv.Key}' not found, skipping enrollments.");
        continue;
    }

    foreach (var (first, last) in kv.Value)
    {
        var student = FindPerson(first, last);
        if (student == null)
        {
            Console.WriteLine($"Warning: student {first} {last} not found, skipping.");
            continue;
        }

        var cs = new CourseStudent { CourseModelId = courseFound.Id, StudentId = student.Id };
        await courseStudentRepo.AddAsync(cs);
    }
}

Console.WriteLine("Seeding complete.\n");

var allPersons = (await personService.ReadAllAsync()).ToList();
Console.WriteLine("Persons (all):");
foreach (var p in allPersons)
{
    var prof = await profileRepo.GetByIdAsync(p.Id); 
    var allProfiles = (await new EfRepository<ProfileModel>(dbFile).GetAllAsync()).ToList();
    var myProfile = allProfiles.FirstOrDefault(x => x.PersonModelId == p.Id);

    Console.WriteLine($"- {p.Id}: {p.FirstName} {p.LastName} (ExternalId={p.ExternalId})");
    if (myProfile != null)
    {
        Console.WriteLine($"    Profile: {myProfile.Bio} (BirthDate={myProfile.BirthDate?.ToShortDateString()})");
    }
}

var allCourses = (await courseService.ReadAllAsync()).ToList();
Console.WriteLine("\nCourses:");
foreach (var c in allCourses)
{
    var instructor = allPersons.FirstOrDefault(x => x.Id == c.InstructorId);
    var instructorName = instructor != null ? $"{instructor.FirstName} {instructor.LastName}" : "(none)";

    var courseStudents = (await courseStudentRepo.GetAllAsync()).Where(cs => cs.CourseModelId == c.Id).ToList();
    var studentNames = new List<string>();
    foreach (var cs in courseStudents)
    {
        var s = allPersons.FirstOrDefault(x => x.Id == cs.StudentId);
        if (s != null) studentNames.Add($"{s.FirstName} {s.LastName}");
    }

    Console.WriteLine($"- {c.Id}: {c.Title} (Credits={c.Credits}) Instructor: {instructorName}");
    Console.WriteLine($"    Enrolled ({studentNames.Count}): {string.Join(", ", studentNames)}");
}

var page1 = await personService.ReadAllAsync(1, 2);
Console.WriteLine("\nPersons page 1:");
foreach (var p in page1)
{
    Console.WriteLine($"- {p.Id}: {p.FirstName} {p.LastName} (ExternalId={p.ExternalId})");
}

var saved = await personService.SaveAsync();
Console.WriteLine($"\nBackup saved: {saved}");

await personService.RemoveAsync(person);
Console.WriteLine("Person removed.");

Console.WriteLine("Demo complete.");
