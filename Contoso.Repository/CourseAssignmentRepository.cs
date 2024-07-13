// MIT License

using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class CourseAssignmentRepository
{
    public static async Task<int> AddAsync(
        CourseAssignment courseAssignment,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                await contosoDbContext
                    .CourseAssignments.AddAsync(courseAssignment)
                    .ConfigureAwait(false);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return courseAssignment.CourseAssignmentId;
            })
            .ConfigureAwait(false);

    public static async Task<List<CourseAssignment>> GetByCourseIdAsync(
        int courseId,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                contosoDbContext = contosoDbContext ?? ContosoDbContextFactory.CreateDbContext();
                return await contosoDbContext
                    .CourseAssignments.Where(c => c.CourseID == courseId)
                    .Include(c => c.Course)
                    .Include(c => c.Instructor)
                    .ToListAsync()
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);
}
