using FakerLib;
using System.Text;

namespace FakerConsole;

class Program
{
    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        
        var faker = new Faker();
        
        PrintValue("int", faker.Create<int>());
        PrintValue("long", faker.Create<long>());
        PrintValue("double", faker.Create<double>());
        PrintValue("string", faker.Create<string>());
        PrintValue("bool", faker.Create<bool>());
        PrintValue("DateTime", faker.Create<DateTime>());
        PrintValue("Guid", faker.Create<Guid>());
        
        try
        {
            var intList = faker.Create<List<int>>();
            Console.WriteLine($"   List<int> ({intList?.Count ?? 0} элементов): [{string.Join(", ", intList ?? new List<int>())}]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка создания List<int>: {ex.Message}");
        }
        
        try
        {
            var stringArray = faker.Create<string[]>();
            Console.WriteLine($"   string[] ({stringArray?.Length ?? 0} элементов): [{string.Join(", ", stringArray ?? new string[0])}]");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка создания string[]: {ex.Message}");
        }
        
        try
        {
            var user = faker.Create<User>();
            if (user != null)
            {
                Console.WriteLine($"   User: Id={user.Id}, Name=\"{user.Name ?? "null"}\", Age={user.Age}, " +
                                 $"Email=\"{user.Email ?? "null"}\", Created={user.CreatedDate:yyyy-MM-dd}, Active={user.IsActive}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"   Ошибка создания User: {ex.Message}");
        }
    }

    static void PrintValue(string name, object value)
    {
        string stringValue = value?.ToString() ?? "null";
        if (value is string)
            stringValue = $"\"{stringValue}\"";
        Console.WriteLine($"   {name}: {stringValue}");
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