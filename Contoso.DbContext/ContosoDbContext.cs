// MIT License
using System.Reflection;
using Contoso.DbContext.Configuration;
using Contoso.Repository.Models;
using Microsoft.EntityFrameworkCore;

public class ContosoDbContext : DbContext
{
    public ContosoDbContext(DbContextOptions<ContosoDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(
            Assembly.GetAssembly(typeof(CourseConfiguration))
        );
    }

    public DbSet<CourseAssignment> CourseAssignments { get; set; }
    public DbSet<Course> Courses { get; set; }
    public DbSet<Department> Departments { get; set; }
    public DbSet<Enrollment> Enrollments { get; set; }
    public DbSet<Instructor> Instructors { get; set; }
    public DbSet<OfficeAssignment> OfficeAssignments { get; set; }
    public DbSet<Student> Students { get; set; }
}
