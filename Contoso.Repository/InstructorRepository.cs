// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class InstructorRepository
{
    public static async Task<int> AddAsync(
        Instructor instructor,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                await contosoDbContext.Instructors.AddAsync(instructor).ConfigureAwait(false);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return instructor.InstructorId;
            })
            .ConfigureAwait(false);

    public static async Task<Unit> DeleteAsync(int id, ContosoDbContext? contosoDbContext = null) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                var instructor =
                    await contosoDbContext.Instructors.FindAsync(id).ConfigureAwait(false)
                    ?? throw new Exception($"{id} does not exist.");
                contosoDbContext.Instructors.Remove(instructor);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return Unit.Default;
            })
            .ConfigureAwait(false);

    public static async Task<Option<Instructor>> GetAsync(
        int id,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery<Option<Instructor>>(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                return await contosoDbContext
                    .Instructors.Include(i => i.CourseAssignments)
                    .ThenInclude(c => c.Course)
                    .SingleOrDefaultAsync(i => i.InstructorId == id)
                    .Map(Optional)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Unit> UpdateAsync(
        Instructor instructor,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                contosoDbContext.Instructors.Update(instructor);
                return await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);
}
