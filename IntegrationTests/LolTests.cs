using CachePlayground.Models;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Http.Extensions;
using System.Net;
using System.Net.Http.Json;

namespace IntegrationTests;

public class LolTests : IClassFixture<DbFixture>
{
    private const string route = "Person";

    private readonly DbFixture fixture;

    public LolTests(DbFixture fixture)
    {
        this.fixture = fixture;
    }

    [Fact]
    public async Task MeatGrinderMK1()
    {
        const string name = "jack";
        const int age = 11;
        var query = BuildQuery(route,
            [
                new(nameof(Person.Name), name),
                new(nameof(Person.Age), age.ToString()),
        ]);

        await this.fixture.Client.PutAsJsonAsync(route, new PersonRequestDto { Name = name, Age = age });

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(this.fixture.Client.GetAsync(query));
        }
        var responses = await Task.WhenAll(tasks);
        foreach (var response in responses)
        {
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }
    }

    [Fact]
    public async Task MeatGrinderMK2()
    {
        const string name = "jack";
        const int age = 11;

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(this.fixture.Client.PutAsJsonAsync(route, new PersonRequestDto { Name = name, Age = age }));
        }
        var responses = await Task.WhenAll(tasks);
        foreach (var response in responses)
        {
            Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
        }
    }

    [Fact]
    public async Task MeatGrinderMK3()
    {
        const string name = "jack";
        const int age = 11;
        var query = BuildQuery(route,
            [
                new(nameof(Person.Name), name),
                new(nameof(Person.Age), age.ToString()),
        ]);

        await this.fixture.Client.PutAsJsonAsync(route, new PersonRequestDto { Name = name, Age = age });

        var tasks = new List<Task<HttpResponseMessage>>();
        for (int i = 0; i < 10; i++)
        {
            tasks.Add(this.fixture.Client.GetAsync(query));
            tasks.Add(this.fixture.Client.PutAsJsonAsync(route, new PersonRequestDto { Name = name, Age = age }));
        }
        var responses = await Task.WhenAll(tasks);
        foreach (var response in responses)
        {
            response.EnsureSuccessStatusCode();
        }
    }

    private static string BuildQuery(string route, KeyValuePair<string, string>[]? arguments)
    {
        if (arguments is null || arguments.Length == 0)
        {
            return route;
        }

        var builder = new QueryBuilder();
        foreach (var arg in arguments)
        {
            builder.Add(arg.Key, arg.Value);
        }

        return route + builder;
    }
}