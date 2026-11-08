using ApiFunkosConjunta.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;

namespace ApiFunkosConjunta.Tests.Data.Seed;

public abstract class PostgresSeederTestBase {
    private readonly PostgreSqlContainer _postgresContainer =
        new PostgreSqlBuilder()
            .WithImage("postgres:18")
            .WithDatabase("ApiFunkosTests")
            .WithUsername("postgres")
            .WithPassword("postgres")
            .Build();

    protected ServiceProvider ServiceProvider = null!;
    protected string ConnectionString => _postgresContainer.GetConnectionString();

    [OneTimeSetUp]
    public async Task OneTimeSetUp() {
        await _postgresContainer.StartAsync();
    }

    [SetUp]
    public async Task SetUp() {
        var services = new ServiceCollection();

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(_postgresContainer.GetConnectionString()));

        ServiceProvider = services.BuildServiceProvider();

        using var scope = ServiceProvider.CreateScope();

        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }

    [TearDown]
    public async Task TearDown() {
        await ServiceProvider.DisposeAsync();
    }

    [OneTimeTearDown]
    public async Task OneTimeTearDown() {
        await _postgresContainer.DisposeAsync();
    }
}