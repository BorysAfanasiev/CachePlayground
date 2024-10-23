using CachePlayground.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace IntegrationTests.Fixtures.Mocks;

internal sealed class TestApplicationFactory : WebApplicationFactory<Program>
{
    private readonly string DbConnectionString;

    public TestApplicationFactory(string dbConnectionString)
    {
        DbConnectionString = dbConnectionString;
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(DbConnectionString));
        });

        base.ConfigureWebHost(builder);
    }
}