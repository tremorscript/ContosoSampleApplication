// MIT License
using Contoso.Core;
using Contoso.Repository;
using Contoso.Repository.Models;
using LanguageExt;
using static LanguageExt.Prelude;

namespace Contoso.Manager;

public record CreateCourseAssignment
{
    public int CourseId { get; init; }

    public int InstructorId { get; init; }
}

public record GetCourseAssignments
{
    public int CourseId { get; init; }
}

public static class CourseAssignmentManager
{
    private static async Task<Validation<Error, Course>> CourseMustExist(
        CreateCourseAssignment create
    ) =>
        (
            await CourseRepository.GetAsync(create.CourseId).ConfigureAwait(false)
        ).ToValidation<Error>($"Course Id {create.CourseId} does not exist.");

    private static async Task<Validation<Error, Instructor>> InstructorMustExist(
        CreateCourseAssignment createCourseAssignment
    ) =>
        (
            await InstructorRepository
                .GetAsync(createCourseAssignment.InstructorId)
                .ConfigureAwait(false)
        ).ToValidation<Error>(
            $"Instructor Id {createCourseAssignment.InstructorId} does not exist."
        );

    private static async Task<int> Persist(CourseAssignment courseAssignment) =>
        (await CourseAssignmentRepository.AddAsync(courseAssignment).ConfigureAwait(false));

    private static async Task<Validation<Error, CourseAssignment>> Validate(
        CreateCourseAssignment createCourseAssignment
    ) =>
        (
            await CourseMustExist(createCourseAssignment).ConfigureAwait(false),
            await InstructorMustExist(createCourseAssignment).ConfigureAwait(false)
        ).Apply(
            (c, i) => new CourseAssignment { CourseID = c.CourseId, InstructorID = i.InstructorId }
        );

    public static async Task<Validation<Error, int>> Create(CreateCourseAssignment request) =>
        await (
            from validationResult in Validate(request)
            let result = validationResult.Match(
                x => Success<Error, int>(Persist(x).Result),
                y => Fail<Error, int>(y)
            )
            select result
        ).ConfigureAwait(false);

    public static async Task<List<CourseAssignment>> GetCourseAssignments(
        GetCourseAssignments request
    ) =>
        await CourseAssignmentRepository.GetByCourseIdAsync(request.CourseId).ConfigureAwait(false);
}
