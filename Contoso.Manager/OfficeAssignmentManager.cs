// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using Contoso.SqlServer;
using LanguageExt;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Manager;

public record CreateOfficeAssignment
{
    public int InstructorId { get; init; }
    public string Location { get; init; }
}

public static class OfficeAssignmentManager
{
    private static async Task<Validation<Error, int>> InstructorMustExist(
        CreateOfficeAssignment create
    ) =>
        from instructor in await InstructorRepository.Get(create.InstructorId).ConfigureAwait(false)
        from result in instructor.ToValidation<Error>(
            $"Student {create.InstructorId} does not exist."
        )
        select result.InstructorId;

    private static async Task<Validation<Error, int>> Persist(OfficeAssignment officeAssignment) =>
        await OfficeAssignmentRepository.Create(officeAssignment).ConfigureAwait(false);

    private static async Task<Validation<Error, OfficeAssignment>> Validate(
        CreateOfficeAssignment create
    ) =>
        (NotEmpty(create.Location), await InstructorMustExist(create).ConfigureAwait(false)).Apply(
            (loc, id) => new OfficeAssignment { InstructorId = id, Location = loc }
        );

    public static async Task<Validation<Error, int>> Create(CreateOfficeAssignment request)
    {
        return await (
            from assignment in Validate(request)
            from result in assignment.Match(Persist, y => Task.FromResult(Fail<Error, int>(y)))
            select result
        ).ConfigureAwait(false);
    }
}
