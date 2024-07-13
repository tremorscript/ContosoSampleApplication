// MIT License

using Contoso.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.DbContext.Configuration;

sealed class StudentConfiguration : IEntityTypeConfiguration<Student>
{
    public void Configure(EntityTypeBuilder<Student> builder)
    {
        builder.Property(b => b.FirstName).HasMaxLength(50);

        builder.Property(b => b.LastName).HasMaxLength(50);
    }
}
