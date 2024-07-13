// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using Contoso.SqlServer;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso.Manager;

public record EnrollmentBase
{
    public Course Course { get; init; }

    public int CourseId { get; init; }

    public Grade? Grade { get; init; }

    public Student Student { get; init; }

    public int StudentId { get; init; }
}

public record CreateEnrollment : EnrollmentBase { }

public record UpdateEnrollment : EnrollmentBase
{
    public int EnrollmentId { get; init; }
}

public static class EnrollmentManager
{
    private static async Task<Validation<Error, Unit>> ApplyUpdate(
        Enrollment enrollment,
        UpdateEnrollment update
    ) =>
        await EnrollmentRepository.Update(
            enrollment with
            {
                EnrollmentId = enrollment.EnrollmentId,
                Grade = update.Grade,
                CourseId = enrollment.CourseId,
                StudentId = enrollment.StudentId
            }
        ).ConfigureAwait(false);

    private static async Task<Validation<Error, int>> CourseMustExist(EnrollmentBase create) =>
        from course in (await CourseRepository.Get(create.CourseId).ConfigureAwait(false))
        from result in course.ToValidation<Error>($"Course {create.CourseId} does not exist.")
        select result.CourseId;

    private static async Task<Validation<Error, Enrollment>> EnrollmentMustExist(
        int enrollmentId
    ) =>
        from enrollment in await EnrollmentRepository.Get(enrollmentId).ConfigureAwait(false)
        from result in enrollment.ToValidation<Error>(
            $"Enrollment Id {enrollmentId} does not exist"
        )
        select result;

    private static async Task<Validation<Error, int>> StudentMustExist(EnrollmentBase create) =>
        from student in await StudentRepository.Get(create.StudentId).ConfigureAwait(false)
        from result in student.ToValidation<Error>($"Student {create.StudentId} does not exist.")
        select result.StudentId;

    private static async Task<Validation<Error, Enrollment>> Validate(EnrollmentBase create) =>
        (
            await CourseMustExist(create).ConfigureAwait(false),
            await StudentMustExist(create).ConfigureAwait(false)
        ).Apply(
            (c, s) =>
                new Enrollment
                {
                    CourseId = c,
                    StudentId = s,
                    Grade = create.Grade
                }
        );

    public static async Task<Validation<Error, int>> Create(CreateEnrollment request) =>
        await (
            from valid in Validate(request)
            from result in valid.Match(
                x => EnrollmentRepository.Add(x),
                x => Task.FromResult(Fail<Error, int>(x))
            )
            select result
        ).ConfigureAwait(false);

    public static async Task<Validation<Error, Unit>> Delete(int enrollmentId) =>
        await (
            from enrollment in EnrollmentMustExist(enrollmentId)
            from result in enrollment.Match(
                e => EnrollmentRepository.Delete(e.EnrollmentId),
                y => Task.FromResult(Fail<Error, Unit>(y))
            )
            select result
        ).ConfigureAwait(false);

    public static async Task<Validation<Error, Enrollment>> GetEnrollmentById(int enrollmentId) =>
        from enrollment in await EnrollmentRepository.Get(enrollmentId).ConfigureAwait(false)
        from result in enrollment.ToValidation<Error>(
            $"Enrollment Id {enrollmentId} does not exist"
        )
        select result;

    public static async Task<Validation<Error, Unit>> Update(UpdateEnrollment request) =>
        await (
            from enrollment in Validate(request)
            from result in enrollment.Match(
                x => ApplyUpdate(x, request),
                x => Task.FromResult(Fail<Error, Unit>(x))
            )
            select Unit.Default
        ).ConfigureAwait(false);
}
