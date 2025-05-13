using FluentMigrator.Runner;

namespace OutboxService.Infrastructure.Database.Migrations;

public static class MigrationRunner
{
    public static void Up(IConfiguration cfg)
    {
        var connectionString = cfg.GetConnectionString("pgConnection");
        if (string.IsNullOrEmpty(connectionString))
            throw new Exception("PostgreSQL Connection string not found");
        
        var runner = CreateServiceProvider(connectionString)
            .GetRequiredService<IMigrationRunner>();
        runner.MigrateUp();
    }
    
    private static ServiceProvider CreateServiceProvider(string connectionString)
    {
        return new ServiceCollection()
            .AddFluentMigratorCore()
            .ConfigureRunner(
                rb => rb.AddPostgres()
                    .WithGlobalConnectionString(connectionString)
                    .ScanIn(typeof(MigrationRunner).Assembly)
                    .For.All()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole())
            .BuildServiceProvider(false);
    }
}