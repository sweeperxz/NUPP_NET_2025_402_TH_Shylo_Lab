namespace University.Infrastructure;

using Microsoft.EntityFrameworkCore;
using University.Infrastructure.Models;

public class UniversityContext : DbContext
{
    public UniversityContext(DbContextOptions<UniversityContext> options) : base(options)
    {
    }

    public DbSet<PersonModel> Persons { get; set; } = null!;
    public DbSet<ProfileModel> Profiles { get; set; } = null!;
    public DbSet<CourseModel> Courses { get; set; } = null!;
    public DbSet<CourseStudent> CourseStudents { get; set; } = null!;


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Person - Profile one-to-one
        modelBuilder.Entity<PersonModel>()
            .HasOne(p => p.Profile)
            .WithOne(pf => pf.Person)
            .HasForeignKey<ProfileModel>(pf => pf.PersonModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // Person - Course one-to-many (Instructor)
        modelBuilder.Entity<CourseModel>()
            .HasOne(c => c.Instructor)
            .WithMany(p => p.Courses)
            .HasForeignKey(c => c.InstructorId)
            .OnDelete(DeleteBehavior.SetNull);

        // CourseStudent join entity (many-to-many)
        modelBuilder.Entity<CourseStudent>()
            .HasKey(cs => new { cs.CourseModelId, cs.StudentId });

        modelBuilder.Entity<CourseStudent>()
            .HasOne(cs => cs.Course)
            .WithMany(c => c.CourseStudents)
            .HasForeignKey(cs => cs.CourseModelId);

        modelBuilder.Entity<CourseStudent>()
            .HasOne(cs => cs.Student)
            .WithMany()
            .HasForeignKey(cs => cs.StudentId);

        base.OnModelCreating(modelBuilder);
    }
}

