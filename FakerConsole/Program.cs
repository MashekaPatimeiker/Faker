using FakerLib;
using System.Text;

namespace FakerConsole;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        var faker = new Faker();
        var regularUser = faker.Create<User>();
        
        Console.WriteLine($"Id: {regularUser.Id}");
        Console.WriteLine($"Name: {regularUser.Name}");
        Console.WriteLine($"Age: {regularUser.Age}");
        Console.WriteLine($"Email: {regularUser.Email}");
        Console.WriteLine($"Created: {regularUser.CreatedDate:yyyy-MM-dd}");
        Console.WriteLine($"Active: {regularUser.IsActive}");
        
        var config = new FakerConfig();
        config.Add<User, string, CityGenerator>(u => u.Name);
        config.Add<User, int, AdultAgeGenerator>(u => u.Age);
        config.Add<User, string, EmailGenerator>(u => u.Email);
        
        var configuredFaker = new Faker(config);
        var configuredUser = configuredFaker.Create<User>();
        
        Console.WriteLine($"Id: {configuredUser.Id}");
        Console.WriteLine($"Name: {configuredUser.Name}");
        Console.WriteLine($"Age: {configuredUser.Age}");
        Console.WriteLine($"Email: {configuredUser.Email}");
        Console.WriteLine($"Created: {configuredUser.CreatedDate:yyyy-MM-dd}");
        Console.WriteLine($"Active: {configuredUser.IsActive}");
        
        var personConfig = new FakerConfig();
        personConfig.Add<ImmutablePerson, string, CityGenerator>(p => p.Name);
        
        var personFaker = new Faker(personConfig);
        var person = personFaker.Create<ImmutablePerson>();
        
        Console.WriteLine($"Name: {person.Name}");
        
        var dateConfig = new FakerConfig();
        dateConfig.Add<Event, DateTime, PastDateGenerator>(e => e.EventDate);
        dateConfig.Add<Event, string, CityGenerator>(e => e.Location);
        
        var dateFaker = new Faker(dateConfig);
        var event_ = dateFaker.Create<Event>();
        
        Console.WriteLine($"Name: {event_.Name}");
        Console.WriteLine($"EventDate: {event_.EventDate:yyyy-MM-dd}");
        Console.WriteLine($"Location: {event_.Location}");
        
        var mixedConfig = new FakerConfig();
        mixedConfig.Add<MixedCasePerson, string, CityGenerator>(p => p.FullName);
        
        var mixedFaker = new Faker(mixedConfig);
        var mixedPerson = mixedFaker.Create<MixedCasePerson>();
        
        Console.WriteLine($"FullName: {mixedPerson.FullName}");
    }
}

public class User
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public int Age { get; set; }
    public string? Email { get; set; }
    public DateTime CreatedDate { get; set; }
    public bool IsActive { get; set; }
}

public class ImmutablePerson
{
    public string Name { get; }
    public ImmutablePerson(string name) => Name = name;
}

public class MixedCasePerson
{
    public string FullName { get; }
    public MixedCasePerson(string fullName) => FullName = fullName;
}

public class Event
{
    public string? Name { get; set; }
    public DateTime EventDate { get; set; }
    public string? Location { get; set; }
}

public class CityGenerator : IValueGenerator
{
    private readonly string[] _cities = { 
        "Moscow", "London", "Paris", "Berlin", "Tokyo", 
        "New York", "Sydney", "Rome", "Madrid", "Kyiv",
        "Amsterdam", "Barcelona", "Prague", "Vienna", "Lisbon"
    };
    
    public object Generate(Type typeToGenerate, GeneratorContext context) =>
        _cities[context.Random.Next(_cities.Length)];

    public bool CanGenerate(Type type) => type == typeof(string);
}

public class AdultAgeGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context) =>
        context.Random.Next(18, 66);

    public bool CanGenerate(Type type) => type == typeof(int);
}

public class EmailGenerator : IValueGenerator
{
    private readonly string[] _domains = { "gmail.com", "yahoo.com", "hotmail.com", "outlook.com", "gayr.net" };
    
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var name = GenerateRandomString(context.Random, context.Random.Next(5, 10));
        var domain = _domains[context.Random.Next(_domains.Length)];
        return $"{name}@{domain}";
    }

    private string GenerateRandomString(Random random, int length)
    {
        const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[random.Next(s.Length)]).ToArray());
    }

    public bool CanGenerate(Type type) => type == typeof(string);
}

public class PastDateGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context) =>
        DateTime.Now.AddDays(-context.Random.Next(1, 1000));

    public bool CanGenerate(Type type) => type == typeof(DateTime);
}