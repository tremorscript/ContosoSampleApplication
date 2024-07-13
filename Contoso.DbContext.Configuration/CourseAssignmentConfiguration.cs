// MIT License

using Contoso.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.DbContext.Configuration;

sealed class CourseAssignmentConfiguration : IEntityTypeConfiguration<CourseAssignment>
{
    public void Configure(EntityTypeBuilder<CourseAssignment> builder)
    {
        builder
            .HasOne(c => c.Course)
            .WithMany(c => c.CourseAssignments)
            .OnDelete(DeleteBehavior.SetNull);

        builder
            .HasOne(c => c.Instructor)
            .WithMany(i => i.CourseAssignments)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
