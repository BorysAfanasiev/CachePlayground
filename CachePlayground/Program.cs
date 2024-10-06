using CachePlayground.Infrastructure;
using CachePlayground.Services;
using CachePlayground.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using Testcontainers.PostgreSql;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

const string user = "admin";
const string password = "12345678";
const string database = "tcDb";

var container = new PostgreSqlBuilder()
    .WithImage("postgres:15-alpine")
    .WithEnvironment("POSTGRES_USER", user)
    .WithEnvironment("POSTGRES_PASSWORD", password)
    .WithEnvironment("POSTGRES_DB", database)
    .Build();
await container.StartAsync();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    var dbConnectionString = new NpgsqlConnectionStringBuilder
    {
        Host = container.Hostname,
        Database = database,
        Port = container.GetMappedPublicPort("5432"),
        Username = user,
        Password = password,
    }.ConnectionString;
    options.UseNpgsql(dbConnectionString);
});
builder.Services.AddMemoryCache();
builder.Services.AddScoped<IPersonService, PersonService>();
builder.Services.AddScoped<IPersonRepository, PersonRepository>();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();

{
    using var scope = app.Services.CreateScope();
    await using var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();