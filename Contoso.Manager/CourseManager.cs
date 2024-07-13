// MIT License

namespace Contoso.Manager;

using Contoso.Core;
using Contoso.Repository;
using Contoso.Repository.Models;
using LanguageExt;
using static Contoso.Validators;
using static LanguageExt.Prelude;

public record CreateCourse
{
    public int Credits { get; init; }

    public int DepartmentId { get; init; }

    public string Title { get; init; }
}

public record DeleteCourse
{
    public int CourseId { get; init; }
}

public record UpdateCourse
{
    public int CourseId { get; init; }

    public int Credits { get; init; }

    public int DepartmentId { get; init; }

    public string Title { get; init; }
}

public record GetCourseById
{
    public int CourseId { get; init; }
}

public static class CourseManager
{
    private static async Task<Validation<Error, Course>> CourseMustExist(
        DeleteCourse deleteCourse
    ) =>
        (
            from course in (
                await CourseRepository.GetAsync(deleteCourse.CourseId).ConfigureAwait(false)
            ).ToValidation<Error>($"Course Id: {deleteCourse.CourseId} does not exist.")
            select course
        );

    private static async Task<Validation<Error, Department>> DepartmentMustExist(
        int departmentId
    ) =>
        (
            from department in await DepartmentManager
                .GetDepartmentById(new GetDepartmentById() { DepartmentId = departmentId })
                .ConfigureAwait(false)
            from validationResult in department.ToValidation<Error>(
                $"Department Id {departmentId} does not exist."
            )
            select validationResult
        );

    private static async Task<Validation<Error, Unit>> DoDeletion(int courseId) =>
        (await CourseRepository.DeleteAsync(courseId).ConfigureAwait(false));

    private static async Task<Validation<Error, Course>> Validate(
        int departmentId,
        string title,
        int credits
    ) =>
        (await DepartmentMustExist(departmentId).ConfigureAwait(false), ValidateTitle(title)).Apply(
            (department, title) =>
                new Course
                {
                    CourseId = 0,
                    Title = title,
                    DepartmentId = department.DepartmentId,
                    Credits = credits
                }
        );

    private static Validation<Error, string> ValidateTitle(string title) =>
        NotEmpty(title).Bind(NotLongerThan(50));

    public static async Task<Validation<Error, int>> CreateCourse(CreateCourse request) =>
        await (
            from course in Validate(request.DepartmentId, request.Title, request.Credits)
            let result = course.Match(
                i => (CourseRepository.AddAsync(i).Result),
                x => Fail<Error, int>(x)
            )
            select result
        ).ConfigureAwait(false);

    public static Task<Validation<Error, Unit>> DeleteCourse(DeleteCourse deleteCourse) =>
        from course in CourseMustExist(deleteCourse)
        from result in course.Match(
            course => DoDeletion(course.CourseId),
            x => Task.FromResult(Fail<Error, Unit>(x))
        )
        select result;

    public static async Task<Validation<Error, Course>> GetCourseById(GetCourseById request) =>
        (
            await CourseRepository.GetAsync(request.CourseId).ConfigureAwait(false)
        ).ToValidation<Error>($"Course Id: {request.CourseId} does not exist.");

    public static Task<Validation<Error, Unit>> UpdateCourse(UpdateCourse updateCourse) =>
        from course in Validate(updateCourse.DepartmentId, updateCourse.Title, updateCourse.Credits)
        from result in course.Match(
            i => ApplyUpdate(i, updateCourse),
            i => Task.FromResult(Fail<Error, Unit>(i))
        )

        select result;

    private static async Task<Validation<Error, Unit>> ApplyUpdate(
        Course course,
        UpdateCourse updateCourse
    ) =>
        await CourseRepository.UpdateAsync(
            course with
            {
                DepartmentId = updateCourse.DepartmentId,
                Credits = updateCourse.Credits,
                Title = updateCourse.Title
            }
        );
}
