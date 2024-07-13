// MIT License

using Contoso.Core;
using Contoso.Repository.Models;
using LanguageExt;
using Microsoft.EntityFrameworkCore;
using static Contoso.Repository.Query;
using static LanguageExt.Prelude;

namespace Contoso.Repository;

public static class StudentRepository
{
    public static async Task<int> AddAsync(Student student) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();
                await contosoDbContext.Students.AddAsync(student).ConfigureAwait(false);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return student.StudentId;
            })
            .ConfigureAwait(false);

    public static async Task<Unit> DeleteAsync(int studentId) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();
                var student =
                    await contosoDbContext.Students.FindAsync(studentId).ConfigureAwait(false)
                    ?? throw new Exception($"{studentId} does not exist.");
                contosoDbContext.Students.Remove(student);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return Unit.Default;
            })
            .ConfigureAwait(false);

    public static async Task<Option<Student>> GetAsync(int id) =>
        await ExecuteQuery<Option<Student>>(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();

                return await contosoDbContext
                    .Students.Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .SingleOrDefaultAsync(s => s.StudentId == id)
                    .Map(Optional)
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<List<Student>> GetAllAsync() =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();
                return await contosoDbContext
                    .Students.Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                    .ToListAsync()
                    .ConfigureAwait(false);
            })
            .ConfigureAwait(false);

    public static async Task<Unit> UpdateAsync(
        Student student,
        ContosoDbContext? contosoDbContext = null
    ) =>
        await ExecuteQuery(async () =>
            {
                using var contosoDbContext = ContosoDbContextFactory.CreateDbContextFunc();
                contosoDbContext.Students.Update(student);
                await contosoDbContext.SaveChangesAsync().ConfigureAwait(false);
                return Unit.Default;
            })
            .ConfigureAwait(false);
}
