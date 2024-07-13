// MIT License

using Microsoft.EntityFrameworkCore;

public static class ContosoDbContextFactory
{
    public static ContosoDbContext CreateDbContext(string? connectionString = null)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ContosoDbContext>();
        // Todo:
        string localConnection =
            connectionString
            ?? "Server=(localdb)\\mssqllocaldb;Database=Contoso;Trusted_Connection=True;Application Name=Contoso;";
        optionsBuilder.UseSqlServer(localConnection);
        return new ContosoDbContext(optionsBuilder.Options);
    }

    public static Func<ContosoDbContext> CreateDbContextFunc(string connectionString)
    {
        return () =>
        {
            var optionsBuilder = new DbContextOptionsBuilder<ContosoDbContext>();
            string localConnection =
                connectionString
                ?? "Server=(localdb)\\mssqllocaldb;Database=Contoso;Trusted_Connection=True;Application Name=Contoso;";
            optionsBuilder.UseSqlServer(localConnection);
            return new ContosoDbContext(optionsBuilder.Options);
        };
    }
}
