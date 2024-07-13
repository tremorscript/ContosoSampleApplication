// MIT License

namespace Contoso.Repository.Models;

public record Course
{
    public List<CourseAssignment> CourseAssignments { get; init; }

    public required int CourseId { get; init; }

    public required int Credits { get; init; }

    public Department Department { get; init; }

    public int? DepartmentId { get; init; }

    public List<Enrollment> Enrollments { get; init; }

    public required string Title { get; init; }
}

public record Department
{
    public Instructor Administrator { get; init; }

    public int? AdministratorId { get; init; }

    public decimal Budget { get; init; }

    public List<Course> Courses { get; init; }

    public int DepartmentId { get; init; }

    public string Name { get; init; }

    public DateTime StartDate { get; init; }
}

public record Enrollment
{
    public Course Course { get; init; }

    public int CourseId { get; init; }

    public int EnrollmentId { get; init; }

    public Grade? Grade { get; init; }

    public Student Student { get; init; }

    public int StudentId { get; init; }
}

public record CourseAssignment
{
    public Course Course { get; init; }

    public int CourseAssignmentId { get; init; }

    public int CourseID { get; init; }

    public Instructor Instructor { get; init; }

    public int InstructorID { get; init; }
}

public record Instructor
{
    public List<CourseAssignment> CourseAssignments { get; init; }

    public string FirstName { get; init; }

    public string FullName => LastName + ", " + FirstName;

    public DateTime HireDate { get; init; }

    public int InstructorId { get; init; }

    public string LastName { get; init; }
}

public enum Grade
{
    A,
    B,
    C,
    D,
    F
}

public record Student
{
    public DateTime EnrollmentDate { get; init; }

    public List<Enrollment> Enrollments { get; init; }

    public string FirstName { get; init; }

    public string FullName => LastName + ", " + FirstName;

    public string LastName { get; init; }

    public int StudentId { get; init; }
}

public class OfficeAssignment
{
    public Instructor Instructor { get; set; }

    public int InstructorId { get; set; }

    public string Location { get; set; }

    public int OfficeAssignmentId { get; set; }
}
