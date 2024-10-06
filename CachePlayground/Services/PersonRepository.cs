using CachePlayground.Infrastructure;
using CachePlayground.Models;
using CachePlayground.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CachePlayground.Services;

public class PersonRepository : IPersonRepository
{
    private readonly AppDbContext ctx;

    public PersonRepository(AppDbContext ctx)
    {
        this.ctx = ctx;
    }

    public async Task<Person?> GetAsync(string name, int age, CancellationToken ct) =>
        await this.ctx.People
            .AsNoTracking()
            .FirstOrDefaultAsync(y => y.Name == name && y.Age == age, ct);

    public async Task PutAsync(Person person, CancellationToken ct)
    {
        await ctx.People.AddAsync(person, ct);
        await this.ctx.SaveChangesAsync(ct);
    }
}