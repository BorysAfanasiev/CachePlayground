using CachePlayground.Models;

namespace CachePlayground.Services.Interfaces;

public interface IPersonRepository
{
    Task<Person?> GetAsync(string name, int age, CancellationToken ct);

    Task PutAsync(Person person, CancellationToken ct);
}