// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class DepartmentRepository
{
    public static async Task<int> AddAsync(Department department) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();

                await contosoDbContext.Departments.AddAsync(department).ConfigureAwait(false);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return department.DepartmentId;
            })
            .ConfigureAwait(false);

    public static async Task<Unit> DeleteAsync(int departmentId) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();
                var department =
                    await contosoDbContext.Departments.FindAsync(departmentId).ConfigureAwait(false)
                    ?? throw new Exception($"{departmentId} not found");
                contosoDbContext.Departments.Remove(department);
                return await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Option<Department>> GetAsync(int id) =>
        await ExecuteQuery<Option<Department>>(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();
                return await contosoDbContext
                    .Departments.SingleOrDefaultAsync(d => d.DepartmentId == id)
                    .Map(Optional)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Unit> UpdateAsync(Department department) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();

                contosoDbContext.Departments.Update(department);
                return await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);
}
