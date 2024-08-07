// MIT License

using Contoso.Core;
using Contoso.Repository;
using Contoso.Repository.Models;
using LanguageExt;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Manager;

public abstract record InstructorBase
{
    public string FirstName { get; init; } = String.Empty;

    public string LastName { get; init; } = String.Empty;
}

public record CreateInstructor : InstructorBase
{
    public DateTime HireDate { get; init; }
}

public record UpdateInstructor : InstructorBase
{
    public int InstructorId { get; init; }
}

public static class InstructorManager
{
    private static async Task<Validation<Error, Unit>> ApplyUpdate(
        Instructor instructor,
        UpdateInstructor updateInstructor
    ) =>
        await InstructorRepository
            .UpdateAsync(
                instructor with
                {
                    FirstName = updateInstructor.FirstName,
                    LastName = updateInstructor.LastName,
                }
            )
            .ConfigureAwait(false);

    private static async Task<Validation<Error, Instructor>> InstructorMustExist(
        int instructorId
    ) =>
        from result in (
            await InstructorRepository.GetAsync(instructorId).ConfigureAwait(false)
        ).ToValidation<Error>($"Instructor with id {instructorId} does not exist")
        select result;

    private static async Task<Validation<Error, int>> PersistInstructor(Instructor instructor) =>
        await InstructorRepository.AddAsync(instructor).ConfigureAwait(false);

    private static Validation<Error, Instructor> Validate(CreateInstructor command) =>
        (ValidateFirstName(command), ValidateLastName(command), ValidateHireDate(command)).Apply(
            (f, s, h) =>
                new Instructor
                {
                    FirstName = f,
                    LastName = s,
                    HireDate = h
                }
        );

    private static Validation<Error, string> ValidateFirstName(InstructorBase createStudent) =>
        NotEmpty(createStudent.FirstName).Bind(firstName => NotLongerThan(50)(firstName));

    private static Validation<Error, DateTime> ValidateHireDate(CreateInstructor createStudent) =>
        createStudent.HireDate > DateTime.Now.AddYears(5)
            ? Fail<Error, DateTime>($"The enrollment date is too far in the future")
            : Success<Error, DateTime>(createStudent.HireDate);

    private static Validation<Error, string> ValidateLastName(InstructorBase createStudent) =>
        NotEmpty(createStudent.LastName).Bind(lastName => NotLongerThan(50)(lastName));

    private static async Task<Validation<Error, Instructor>> ValidateUpdate(
        UpdateInstructor command
    ) =>
        (
            ValidateFirstName(command),
            ValidateLastName(command),
            await InstructorMustExist(command.InstructorId).ConfigureAwait(false)
        ).Apply((f, s, instructor) => instructor);

    public static async Task<Validation<Error, int>> Create(CreateInstructor request) =>
        await (
            from instructor in Task.FromResult(Validate(request))
            from result in instructor.MatchAsync(async x => await PersistInstructor(x), y => y)
            select result
        ).ConfigureAwait(false);

    public static async Task<Validation<Error, Unit>> Delete(int instructorId) =>
        await (
            from instructor in InstructorMustExist(instructorId)
            from result in instructor.MatchAsync(
                async x =>
                    Validation<Error, Unit>.Success(
                        await InstructorRepository.DeleteAsync(x.InstructorId)
                    ),
                y => y
            )
            select result
        ).ConfigureAwait(false);

    public static async Task<Validation<Error, Option<Instructor>>> GetInstructorById(
        int instructorId
    ) => await InstructorRepository.GetAsync(instructorId).ConfigureAwait(false);

    public static async Task<Validation<Error, Unit>> Update(UpdateInstructor request) =>
        await (
            from instructor in ValidateUpdate(request)
            from result in instructor.Match(
                i => ApplyUpdate(i, request),
                i => Task.FromResult(Fail<Error, Unit>(i))
            )
            select result
        ).ConfigureAwait(false);
}
