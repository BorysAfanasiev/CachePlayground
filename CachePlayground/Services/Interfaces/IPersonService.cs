using CachePlayground.Models;

namespace CachePlayground.Services.Interfaces;

public interface IPersonService
{
    ValueTask<Person?> GetAsync(string name, int age, CancellationToken ct);

    Task PutAsync(PersonRequestDto request, CancellationToken ct);
}