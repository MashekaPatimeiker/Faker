using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakerLib;

namespace FakerTests;

[TestClass]
public class FakerTests
{
    private Faker _faker = null!;

    [TestInitialize]
    public void Setup()
    {
        _faker = new Faker();
    }

    [TestMethod]
    public void Create_Int_ReturnsInt()
    {
        var result = _faker.Create<int>();
        Assert.IsInstanceOfType(result, typeof(int));
    }

    [TestMethod]
    public void Create_String_ReturnsString()
    {
        var result = _faker.Create<string>();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    public void Create_DateTime_ReturnsDateTime()
    {
        var result = _faker.Create<DateTime>();
        Assert.IsInstanceOfType(result, typeof(DateTime));
    }

    [TestMethod]
    public void Create_ListInt_ReturnsList()
    {
        var result = _faker.Create<List<int>>();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<int>));
    }

    [TestMethod]
    public void Create_User_ReturnsUserWithProperties()
    {
        var user = _faker.Create<User>();
        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void Create_WithCircularDependencies_NoStackOverflow()
    {
        var a = _faker.Create<ClassA>();
        Assert.IsNotNull(a);
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

public class ClassA
{
    public string? Value { get; set; } = "A";
    public ClassB? B { get; set; }
}

public class ClassB
{
    public string? Value { get; set; } = "B";
    public ClassC? C { get; set; }
}

public class ClassC
{
    public string? Value { get; set; } = "C";
    public ClassA? A { get; set; }
}