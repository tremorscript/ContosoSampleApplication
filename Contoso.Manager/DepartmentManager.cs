// MIT License

using Contoso.Core;
using Contoso.Repository;
using Contoso.Repository.Models;
using LanguageExt;
using static Contoso.Validators;
using static LanguageExt.Prelude;

namespace Contoso.Manager;

public record GetDepartmentById
{
    public int DepartmentId { get; init; }
}

public record DepartmentBase
{
    public int AdministratorId { get; init; }

    public decimal Budget { get; init; }

    public string Name { get; init; }

    public DateTime StartDate { get; init; }
}

public record CreateDepartment : DepartmentBase { }

public record UpdateDepartment : DepartmentBase
{
    public int DepartmentId { get; init; }
}

public static class DepartmentManager
{
    private static async Task<Validation<Error, Unit>> ApplyUpdateRequest(
        Department department,
        UpdateDepartment update
    ) =>
        await DepartmentRepository
            .UpdateAsync(
                department with
                {
                    Name = update.Name,
                    Budget = update.Budget,
                    AdministratorId = update.AdministratorId,
                    StartDate = update.StartDate
                }
            )
            .ConfigureAwait(false);

    private static async Task<Validation<Error, Department>> DepartmentMustExist(
        int departmentId
    ) =>
        from department in (await DepartmentRepository
            .GetAsync(departmentId)
            .ConfigureAwait(false))
            .ToValidation<Error>($"Department Id {departmentId} does not exist.")
        select department;

    private static async Task<Validation<Error, Instructor>> InstructorIdMustExist(
        DepartmentBase createDepartment
    ) =>
        (
            await InstructorManager
                .GetInstructorById(createDepartment.AdministratorId)
                .ConfigureAwait(false)
        ).Bind(x =>
            x.ToValidation<Error>(
                $"Administrator Id {createDepartment.AdministratorId} does not exist"
            )
        );

    private static Validation<Error, DateTime> MustStartInFuture(DepartmentBase createDepartment) =>
        createDepartment.StartDate > DateTime.UtcNow
            ? Success<Error, DateTime>(createDepartment.StartDate)
            : Fail<Error, DateTime>($"Start date must not be in the past");

    private static async Task<Validation<Error, Department>> Validate(DepartmentBase create) =>
        (
            ValidateDepartmentName(create),
            ValidateBudget(create),
            MustStartInFuture(create),
            await InstructorIdMustExist(create).ConfigureAwait(false)
        ).Apply(
            (n, b, s, i) =>
                new Department
                {
                    Name = n,
                    Budget = b,
                    StartDate = s,
                    AdministratorId = i.InstructorId
                }
        );

    private static Validation<Error, decimal> ValidateBudget(DepartmentBase createDepartment) =>
        AtLeast(0M)(createDepartment.Budget).Bind(AtMost(999999));

    private static Validation<Error, string> ValidateDepartmentName(
        DepartmentBase createDepartment
    ) => NotEmpty(createDepartment.Name).Bind(NotLongerThan(50));

    public static async Task<Validation<Error, int>> Create(CreateDepartment request) =>
        await (
            from department in Validate(request)
            from result in department.Match(
                i => DepartmentRepository.Add(i),
                x => Task.FromResult(Fail<Error, int>(x))
            )
            select result
        ).ConfigureAwait(false);

    public static async Task<Validation<Error, Unit>> Delete(int departmentId) =>
        await (
            from department in DepartmentMustExist(departmentId)
            from result in department.Match(
                i => DepartmentRepository.Delete(departmentId),
                x => Task.FromResult(Fail<Error, Unit>(x))
            )
            select result
        ).ConfigureAwait(false);

    public static async Task<Validation<Error, Option<Department>>> GetDepartmentById(
        GetDepartmentById getDepartmentById
    ) => await DepartmentRepository.GetAsync(getDepartmentById.DepartmentId).ConfigureAwait(false);

    public static async Task<Validation<Error, Unit>> Update(UpdateDepartment request) =>
        await (
            from department in Validate(request)
            from result in department.Match(
                i => ApplyUpdateRequest(i, request),
                i => Task.FromResult(Fail<Error, Unit>(i))
            )
            select result
        ).ConfigureAwait(false);
}
