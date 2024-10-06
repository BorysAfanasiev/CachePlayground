namespace CachePlayground.Models;

public sealed class Person
{
    public int Id { get; set; }
    required public string Name { get; set; }
    required public int Age { get; set; }
}