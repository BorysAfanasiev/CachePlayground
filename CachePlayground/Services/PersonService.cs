using CachePlayground.Models;
using CachePlayground.Services.Interfaces;
using Microsoft.Extensions.Caching.Memory;

namespace CachePlayground.Services;

public class PersonService : IPersonService
{
    private readonly IPersonRepository repo;
    private readonly IMemoryCache cache;
    private readonly ILogger<PersonRepository> logger;

    public PersonService(IPersonRepository repo, IMemoryCache cache, ILogger<PersonRepository> logger)
    {
        this.repo = repo;
        this.cache = cache;
        this.logger = logger;
    }

    public async ValueTask<Person?> GetAsync(string name, int age, CancellationToken ct)
    {
        var key = BuildKey(name, age);
        if (this.cache.TryGetValue(key, out Person? found))
        {
            logger.LogInformation("Acquiered from cache: {key}", key);
            return found;
        }

        var person = await this.repo.GetAsync(name, age, ct);
        if (person is null)
        {
            return null;
        }
        this.cache.Set(key, person);
        logger.LogInformation("Acquiered from db: {key}", key);
        return person;
    }

    public async Task PutAsync(PersonRequestDto request, CancellationToken ct)
    {
        var person = new Person { Name = request.Name, Age = request.Age};
        await this.repo.PutAsync(person, ct);

        var key = BuildKey(request.Name, request.Age);
        this.cache.Set(key, person);
    }

    private static string BuildKey(string name, int age) =>
        Constants.PersonCacheKey + "_" + name + "_" + age;
}