using Microsoft.EntityFrameworkCore;
using University.Infrastructure;
using University.Infrastructure.Models;

var builder = WebApplication.CreateBuilder(args);

// Реєстрація DbContext для SQLite
builder.Services.AddDbContext<UniversityContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Реєстрація репозиторіїв
builder.Services.AddScoped(typeof(IRepository<>), typeof(EfRepository<>));

// Реєстрація CRUD сервісів
builder.Services.AddScoped<ICrudServiceAsync<University.Infrastructure.Models.PersonModel>>(provider =>
{
    var repo = provider.GetRequiredService<IRepository<University.Infrastructure.Models.PersonModel>>();
    var backupPath = Path.Combine(AppContext.BaseDirectory, "backups", "persons.json");
    return new CrudServiceAsync<University.Infrastructure.Models.PersonModel>(repo, backupPath);
});

builder.Services.AddScoped<ICrudServiceAsync<University.Infrastructure.Models.CourseModel>>(provider =>
{
    var repo = provider.GetRequiredService<IRepository<University.Infrastructure.Models.CourseModel>>();
    var backupPath = Path.Combine(AppContext.BaseDirectory, "backups", "courses.json");
    return new CrudServiceAsync<University.Infrastructure.Models.CourseModel>(repo, backupPath);
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<UniversityContext>();

    dbContext.Database.EnsureDeleted();
    dbContext.Database.EnsureCreated();

    // ===========================
    //      SEED TEST DATA
    // ===========================
    if (!dbContext.Persons.Any())
    {
        var p1 = new PersonModel { FirstName = "John", LastName = "Doe" };
        var p2 = new PersonModel { FirstName = "Alice", LastName = "Smith" };
        var p3 = new PersonModel { FirstName = "Bob", LastName = "Johnson" };
        var p4 = new PersonModel { FirstName = "Eva", LastName = "Martinez" };

        dbContext.Persons.AddRange(p1, p2, p3, p4);
        dbContext.SaveChanges();

        // Курсы (p1 — преподаватель)
        var c1 = new CourseModel
        {
            Title = "Introduction to Programming",
            Credits = 5,
            InstructorId = p1.Id
        };

        var c2 = new CourseModel
        {
            Title = "Advanced Databases",
            Credits = 4,
            InstructorId = p2.Id
        };

        var c3 = new CourseModel
        {
            Title = "Software Architecture",
            Credits = 3,
            InstructorId = p1.Id
        };

        dbContext.Courses.AddRange(c1, c2, c3);
        dbContext.SaveChanges();

        // Many-to-many: Students in courses
        dbContext.CourseStudents.AddRange(
            new CourseStudent { CourseModelId = c1.Id, StudentId = p3.Id },
            new CourseStudent { CourseModelId = c1.Id, StudentId = p4.Id },

            new CourseStudent { CourseModelId = c2.Id, StudentId = p3.Id },

            new CourseStudent { CourseModelId = c3.Id, StudentId = p2.Id },
            new CourseStudent { CourseModelId = c3.Id, StudentId = p4.Id }
        );
        var p11 = new PersonModel { FirstName = "John", LastName = "Doe" };
        var p22 = new PersonModel { FirstName = "Alice", LastName = "Smith" };
        var p33 = new PersonModel { FirstName = "Bob", LastName = "Johnson" };
        var p44 = new PersonModel { FirstName = "Eva", LastName = "Martinez" };

        dbContext.Persons.AddRange(p11, p22, p33, p44);
        dbContext.SaveChanges();
    }
}

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();