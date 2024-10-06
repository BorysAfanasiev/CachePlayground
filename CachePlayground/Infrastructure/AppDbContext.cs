using CachePlayground.Models;
using Microsoft.EntityFrameworkCore;

namespace CachePlayground.Infrastructure;

public class AppDbContext(DbContextOptions<AppDbContext> options)
    : DbContext(options)
{
    public DbSet<Person> People { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Person>().HasKey(static x => x.Id);

        base.OnModelCreating(modelBuilder);
    }
}