// MIT License
using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class CourseRepository
{
    public static async Task<int> AddAsync(
        Course course,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery<int>(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                await contosoDbContext.Courses.AddAsync(course).ConfigureAwait(false);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return course.CourseId;
            })
            .ConfigureAwait(false);

    public static async Task<Unit> DeleteAsync(int id, ContosoDbContext? contosoDbContext = null) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                var course =
                    await contosoDbContext.Courses.FindAsync(id).ConfigureAwait(false)
                    ?? throw new Exception($"{id} not found");
                contosoDbContext.Courses.Remove(course);
                return await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Option<Course>> GetAsync(
        int id,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                var result = await contosoDbContext
                    .Courses.SingleOrDefaultAsync(c => c.CourseId == id)
                    .ConfigureAwait(false);
                return Optional(result);
            })
            .ConfigureAwait(false);

    public static async Task<Unit> UpdateAsync(
        Course course,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                contosoDbContext.Courses.Update(course);
                return await contosoDbContext
                    .SaveChangesAsync()
                    .Map(_ => Unit.Default)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);
}
