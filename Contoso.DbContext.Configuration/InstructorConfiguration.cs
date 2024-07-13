// MIT License

using Contoso.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.DbContext.Configuration;

public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
{
    public void Configure(EntityTypeBuilder<Instructor> builder)
    {
        builder.Property(b => b.FirstName).HasMaxLength(50);

        builder.Property(b => b.LastName).HasMaxLength(50);
    }
}
