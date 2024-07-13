// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class OfficeAssignmentRepository
{
    public static async Task<int> CreateAsync(OfficeAssignment officeAssignment) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();

                await contosoDbContext
                    .OfficeAssignments.AddAsync(officeAssignment)
                    .ConfigureAwait(false);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return officeAssignment.OfficeAssignmentId;
            })
            .ConfigureAwait(false);

    public static async Task<Option<OfficeAssignment>> GetByInstructorIdAsync(int instructorId) =>
        await ExecuteQuery<Option<OfficeAssignment>>(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();

                return await contosoDbContext
                    .OfficeAssignments.SingleOrDefaultAsync(o => o.InstructorId == instructorId)
                    .Map(Optional)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);
}
