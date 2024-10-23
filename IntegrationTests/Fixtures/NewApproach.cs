using IntegrationTests.Fixtures.Mocks;
using IntegrationTests.Fixtures.Tools;
using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace IntegrationTests.Fixtures;

#pragma warning disable CS8618

internal class NewApproach : IAsyncLifetime
{
    private PostgreSqlContainer container;
    private WebApplicationFactory<Program> webApplicationFactory;

    protected HttpClientTestFacade Client { get; private set; }

    public async Task InitializeAsync()
    {
        this.container = new PostgreSqlBuilder()
             .WithImage("postgres:15-alpine")
             .WithUsername("admin")
             .WithPassword("12345678")
             .WithDatabase("tcDb")
             .Build();
        await this.container.StartAsync();

        this.webApplicationFactory = new TestApplicationFactory(this.container.GetConnectionString());

        this.Client = new(this.webApplicationFactory.CreateClient());
    }

    public async Task DisposeAsync()
    {
        this.Client.Dispose();
        await this.webApplicationFactory.DisposeAsync();
        await this.container.DisposeAsync();
    }
}