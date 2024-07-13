// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class EnrollmentRepository
{
    public static async Task<int> AddAsync(
        Enrollment enrollment,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                contosoDbContext.Enrollments.Add(enrollment);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return enrollment.EnrollmentId;
            })
            .ConfigureAwait(false);

    public static async Task<Unit> DeleteAsync(int id, ContosoDbContext? contosoDbContext = null) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                var enrollment =
                    await contosoDbContext.Enrollments.FindAsync(id).ConfigureAwait(false)
                    ?? throw new Exception($"{id} not found");
                contosoDbContext.Enrollments.Remove(enrollment);
                return await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Option<Enrollment>> GetAsync(
        int id,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery<Option<Enrollment>>(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                return await contosoDbContext
                    .Enrollments.Include(e => e.Student)
                    .Include(e => e.Course)
                    .SingleOrDefaultAsync(e => e.EnrollmentId == id)
                    .Map(Optional)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Unit> UpdateAsync(
        Enrollment enrollment,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                contosoDbContext.Enrollments.Update(enrollment);
                await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(true);
                return Unit.Default;
            })
            .ConfigureAwait(false);
}
