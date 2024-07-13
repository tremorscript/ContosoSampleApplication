// MIT License

using LanguageExt;
using Microsoft.EntityFrameworkCore;

public static class ContosoDbContextFactory
{
    public static Func<ContosoDbContext> CreateDbContextFunc { get; set; }

    // public static ContosoDbContext CreateDbContext(string? connectionString = null)
    // {
    //     var optionsBuilder = new DbContextOptionsBuilder<ContosoDbContext>();
    //     // Todo:
    //     string localConnection =
    //         connectionString
    //         ?? "Server=(localdb)\\mssqllocaldb;Database=Contoso;Trusted_Connection=True;Application Name=Contoso;";
    //     optionsBuilder.UseSqlServer(localConnection);
    //     return new ContosoDbContext(optionsBuilder.Options);
    // }

    public static Unit SetupDbContext(
        string connectionString,
        DbContextOptions<ContosoDbContext> options
    )
    {
        CreateDbContextFunc = () =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContosoDbContext>();
            string localConnection =
                connectionString
                ?? "Server=(localdb)\\mssqllocaldb;Database=Contoso;Trusted_Connection=True;Application Name=Contoso;";
            optionsBuilder.UseSqlServer(localConnection);
            return new ContosoDbContext(optionsBuilder.Options);
        };

        return Unit.Default;
    }
}
