// MIT License

using Contoso.Repository.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Contoso.DbContext.Configuration;

public class OfficeAssignmentConfiguration : IEntityTypeConfiguration<OfficeAssignment>
{
    public void Configure(EntityTypeBuilder<OfficeAssignment> builder)
    {
        builder.Property(b => b.Location).HasMaxLength(50);
    }
}
