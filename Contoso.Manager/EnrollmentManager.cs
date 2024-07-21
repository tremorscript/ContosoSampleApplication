// MIT License

using Contoso.Core;
using Contoso.Repository;
using Contoso.Repository.Models;
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
        await EnrollmentRepository
            .UpdateAsync(
                enrollment with
                {
                    EnrollmentId = enrollment.EnrollmentId,
                    Grade = update.Grade,
                    CourseId = enrollment.CourseId,
                    StudentId = enrollment.StudentId
                }
            )
            .ConfigureAwait(false);

    private static async Task<Validation<Error, int>> CourseMustExist(EnrollmentBase create) =>
        from course in (
            await CourseRepository.GetAsync(create.CourseId).ConfigureAwait(false)
        ).ToValidation<Error>($"Course {create.CourseId} does not exist.")
        select course.CourseId;

    private static async Task<Validation<Error, Enrollment>> EnrollmentMustExist(
        int enrollmentId
    ) =>
        from enrollment in (
            await EnrollmentRepository.GetAsync(enrollmentId).ConfigureAwait(false)
        ).ToValidation<Error>($"Enrollment Id {enrollmentId} does not exist")
        select enrollment;

    private static async Task<Validation<Error, int>> StudentMustExist(EnrollmentBase create) =>
        from student in (
            await StudentRepository.GetAsync(create.StudentId).ConfigureAwait(false)
        ).ToValidation<Error>($"Student {create.StudentId} does not exist.")
        select student.StudentId;

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
            from result in valid.MatchAsync(async x => await CreateEnrollment(x), y => y)
            select result
        ).ConfigureAwait(false);

    private static async Task<Validation<Error, int>> CreateEnrollment(Enrollment x)
    {
        return await EnrollmentRepository.AddAsync(x);
    }

    public static async Task<Validation<Error, Unit>> Delete(int enrollmentId) =>
        await (
            from enrollment in EnrollmentMustExist(enrollmentId)
            from result in enrollment.MatchAsync(async e => await DeleteEnrollment(e), y => y)
            select result
        ).ConfigureAwait(false);

    private static async Task<Validation<Error, Unit>> DeleteEnrollment(Enrollment enrollment)
    {
        return await EnrollmentRepository.DeleteAsync(enrollment.EnrollmentId);
    }

    public static async Task<Validation<Error, Enrollment>> GetEnrollmentById(int enrollmentId) =>
        from enrollment in (
            await EnrollmentRepository.GetAsync(enrollmentId).ConfigureAwait(false)
        ).ToValidation<Error>($"Enrollment Id {enrollmentId} does not exist")
        select enrollment;

    public static async Task<Validation<Error, Unit>> Update(UpdateEnrollment request) =>
        await (
            from enrollment in Validate(request)
            from result in enrollment.MatchAsync(async x => await ApplyUpdate(x, request), y => y)
            select Unit.Default
        ).ConfigureAwait(false);
}
