// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using Contoso.SqlServer;
using LanguageExt;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Manager;

public abstract record StudentBase
{
    public string FirstName { get; init; } = String.Empty;

    public string LastName { get; init; } = String.Empty;
}

public record CreateStudent : StudentBase
{
    public DateTime EnrollmentDate { get; init; }
}

public record UpdateStudent : StudentBase
{
    public int StudentId { get; }
}

public static class StudentManager
{
    private static async Task<Validation<Error, Unit>> ApplyUpdateRequest(
        Student student,
        UpdateStudent update
    ) =>
        await StudentRepository
            .Update(student with { FirstName = update.FirstName, LastName = update.LastName })
            .ConfigureAwait(false);

    private static async Task<Validation<Error, Unit>> DoDeletion(int studentId) =>
        await StudentRepository.Delete(studentId).ConfigureAwait(false);

    private static Validation<Error, string> NotEmpty(string str) =>
        string.IsNullOrEmpty(str)
            ? Fail<Error, string>("Must not be empty")
            : Success<Error, string>(str);

    private static async Task<Validation<Error, int>> PersistStudent(Student s) =>
        await StudentRepository.Add(s).ConfigureAwait(false);

    private static async Task<Validation<Error, int>> StudentMustExist(int studentId) =>
        from student in await StudentRepository.Get(studentId).ConfigureAwait(false)
        from result in student.ToValidation<Error>($"Student {studentId} does not exist.")
        select result.StudentId;

    private static async Task<Validation<Error, Student>> StudentMustExist(
        UpdateStudent updateStudent
    ) =>
        from student in await StudentRepository.Get(updateStudent.StudentId).ConfigureAwait(false)
        from result in student.ToValidation<Error>(
            $"Student {updateStudent.StudentId} does not exist."
        )
        select result;

    private static Validation<Error, Student> ValidateCreate(CreateStudent request) =>
        (
            ValidateFirstName(request),
            ValidateLastName(request),
            ValidateEnrollmentDate(request)
        ).Apply(
            (f, l, e) =>
                new Student
                {
                    FirstName = f,
                    LastName = l,
                    EnrollmentDate = e
                }
        );

    private static Validation<Error, DateTime> ValidateEnrollmentDate(
        CreateStudent createStudent
    ) =>
        createStudent.EnrollmentDate > DateTime.Now.AddYears(5)
            ? Fail<Error, DateTime>($"The enrollment date is too far in the future")
            : Success<Error, DateTime>(createStudent.EnrollmentDate);

    private static Validation<Error, string> ValidateFirstName(StudentBase createStudent) =>
        NotEmpty(createStudent.FirstName).Bind(firstName => NotLongerThan(50)(firstName));

    private static Validation<Error, string> ValidateLastName(StudentBase createStudent) =>
        NotEmpty(createStudent.LastName).Bind(lastName => NotLongerThan(50)(lastName));

    private static async Task<Validation<Error, Student>> ValidateUpdate(UpdateStudent request) =>
        (
            ValidateFirstName(request),
            ValidateLastName(request),
            await StudentMustExist(request).ConfigureAwait(false)
        ).Apply((first, last, studentToUpdate) => studentToUpdate);

    public static async Task<Validation<Error, int>> Create(CreateStudent request) =>
        await (
            from student in ValidateCreate(request)
                .Match(PersistStudent, x => Task.FromResult(Fail<Error, int>(x)))
            select student
        ).ConfigureAwait(false);

    public static Task<Validation<Error, Unit>> Delete(int studentId)
    {
        return StudentMustExist(studentId)
            .Bind(x => x.Match(DoDeletion, y => Task.FromResult(Fail<Error, Unit>(y))));
    }

    public static Task<Validation<Error, Unit>> Update(UpdateStudent request)
    {
        return ValidateUpdate(request)
            .Bind(v =>
                v.Match(
                    s => ApplyUpdateRequest(s, request),
                    y => Task.FromResult(Fail<Error, Unit>(y))
                )
            );
    }
}
