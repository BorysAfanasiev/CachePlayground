using CachePlayground.Infrastructure;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Fixtures;

#pragma warning disable CS8618

public class DbFixture : IAsyncLifetime
{
    private PostgreSqlContainer container;
    private WebApplicationFactory<Program> webApplicationFactory;

    public HttpClient Client { get; private set; }

    public async Task InitializeAsync()
    {
        const string user = "admin";
        const string password = "12345678";
        const string database = "tcDb";

        this.container = new PostgreSqlBuilder()
             .WithImage("postgres:15-alpine")
             .WithEnvironment("POSTGRES_USER", user)
             .WithEnvironment("POSTGRES_PASSWORD", password)
             .WithEnvironment("POSTGRES_DB", database)
             .Build();
        await this.container.StartAsync();

        this.webApplicationFactory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var dbConnectionString = new NpgsqlConnectionStringBuilder
                    {
                        Host = this.container.Hostname,
                        Database = database,
                        Port = container.GetMappedPublicPort("5432"),
                        Username = user,
                        Password = password,
                    }.ConnectionString;

                    services.AddDbContext<AppDbContext>(options => options.UseNpgsql(dbConnectionString));
                });
            });

        this.Client = this.webApplicationFactory.CreateClient();
    }

    public async Task DisposeAsync()
    {
        this.Client.Dispose();
        await this.webApplicationFactory.DisposeAsync();
        await this.container.DisposeAsync();
    }
}