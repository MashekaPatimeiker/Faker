using Microsoft.VisualStudio.TestTools.UnitTesting;
using FakerLib;
using System;
using System.Collections.Generic;

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
    public void IntTest()
    {
        var result = _faker.Create<int>();
        Assert.IsInstanceOfType(result, typeof(int));
    }

    [TestMethod]
    public void LongTest()
    {
        var result = _faker.Create<long>();
        Assert.IsInstanceOfType(result, typeof(long));
    }

    [TestMethod]
    public void DoubleTest()
    {
        var result = _faker.Create<double>();
        Assert.IsInstanceOfType(result, typeof(double));
    }

    [TestMethod]
    public void FloatTest()
    {
        var result = _faker.Create<float>();
        Assert.IsInstanceOfType(result, typeof(float));
    }

    [TestMethod]
    public void BoolTest()
    {
        var result = _faker.Create<bool>();
        Assert.IsInstanceOfType(result, typeof(bool));
    }

    [TestMethod]
    public void StringTest()
    {
        var result = _faker.Create<string>();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(string));
    }

    [TestMethod]
    public void DateTimeTest()
    {
        var result = _faker.Create<DateTime>();
        Assert.IsInstanceOfType(result, typeof(DateTime));
    }

    [TestMethod]
    public void GuidTest()
    {
        var result = _faker.Create<Guid>();
        Assert.IsInstanceOfType(result, typeof(Guid));
    }

    [TestMethod]
    public void DecimalTest()
    {
        var result = _faker.Create<decimal>();
        Assert.IsInstanceOfType(result, typeof(decimal));
    }

    [TestMethod]
    public void CharTest()
    {
        var result = _faker.Create<char>();
        Assert.IsInstanceOfType(result, typeof(char));
    }

    [TestMethod]
    public void ByteTest()
    {
        var result = _faker.Create<byte>();
        Assert.IsInstanceOfType(result, typeof(byte));
    }

    [TestMethod]
    public void ListIntTest()
    {
        var result = _faker.Create<List<int>>();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(List<int>));
    }

    [TestMethod]
    public void ArrayIntTest()
    {
        var result = _faker.Create<int[]>();
        Assert.IsNotNull(result);
        Assert.IsInstanceOfType(result, typeof(int[]));
    }

    [TestMethod]
    public void ListStringTest()
    {
        var result = _faker.Create<List<string>>();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void NestedListTest()
    {
        var result = _faker.Create<List<List<int>>>();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void UserTest()
    {
        var user = _faker.Create<User>();
        Assert.IsNotNull(user);
    }

    [TestMethod]
    public void ProductTest()
    {
        var product = _faker.Create<Product>();
        Assert.IsNotNull(product);
    }

    [TestMethod]
    public void MultipleConstructorsTest()
    {
        var obj = _faker.Create<MultipleConstructorsClass>();
        Assert.IsNotNull(obj);
    }

    [TestMethod]
    public void StructTest()
    {
        var point = _faker.Create<PointStruct>();
        Assert.IsInstanceOfType(point, typeof(PointStruct));
    }

    [TestMethod]
    public void StructWithConstructorTest()
    {
        var rect = _faker.Create<RectangleStruct>();
        Assert.IsInstanceOfType(rect, typeof(RectangleStruct));
    }

    [TestMethod]
    public void CircularDependenciesTest()
    {
        var a = _faker.Create<ClassA>();
        Assert.IsNotNull(a);
    }

    [TestMethod]
    public void DeepCircularTest()
    {
        var root = _faker.Create<CircularRoot>();
        Assert.IsNotNull(root);
    }

    [TestMethod]
    public void UniquenessIntTest()
    {
        var val1 = _faker.Create<int>();
        var val2 = _faker.Create<int>();
        Assert.AreNotEqual(val1, val2);
    }

    [TestMethod]
    public void UniquenessUserTest()
    {
        var user1 = _faker.Create<User>();
        var user2 = _faker.Create<User>();
        Assert.AreNotEqual(user1.Id, user2.Id);
        Assert.AreNotEqual(user1.Name, user2.Name);
    }

    [TestMethod]
    public void NullableTest()
    {
        var result = _faker.Create<int?>();
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public void ConfigTest()
    {
        var config = new FakerConfig();
        config.Add<User, string, TestCityGenerator>(u => u.Name);
        
        var faker = new Faker(config);
        var user = faker.Create<User>();
        
        Assert.AreEqual("TestCity", user.Name);
    }

    [TestMethod]
    public void MultipleConfigTest()
    {
        var config = new FakerConfig();
        config.Add<User, string, TestCityGenerator>(u => u.Name);
        config.Add<User, int, TestAgeGenerator>(u => u.Age);
        config.Add<User, string, TestEmailGenerator>(u => u.Email);
        
        var faker = new Faker(config);
        var user = faker.Create<User>();
        
        Assert.AreEqual("TestCity", user.Name);
        Assert.AreEqual(25, user.Age);
        Assert.AreEqual("test@test.com", user.Email);
    }

    [TestMethod]
    public void ImmutablePersonTest()
    {
        var config = new FakerConfig();
        config.Add<ImmutablePerson, string, TestCityGenerator>(p => p.Name);
        
        var faker = new Faker(config);
        var person = faker.Create<ImmutablePerson>();
        
        Assert.IsNotNull(person);
        Assert.AreEqual("TestCity", person.Name);
    }

    [TestMethod]
    public void MixedCasePersonTest()
    {
        var config = new FakerConfig();
        config.Add<MixedCasePerson, string, TestCityGenerator>(p => p.FullName);
        
        var faker = new Faker(config);
        var person = faker.Create<MixedCasePerson>();
        
        Assert.IsNotNull(person);
        Assert.AreEqual("TestCity", person.FullName);
    }

    [TestMethod]
    public void EventConfigTest()
    {
        var config = new FakerConfig();
        config.Add<Event, DateTime, TestPastDateGenerator>(e => e.EventDate);
        config.Add<Event, string, TestCityGenerator>(e => e.Location);
        
        var faker = new Faker(config);
        var event_ = faker.Create<Event>();
        
        Assert.IsNotNull(event_);
        Assert.AreEqual("TestCity", event_.Location);
        Assert.IsTrue(event_.EventDate < DateTime.Now);
    }

    [TestMethod]
    public void NoExceptionsTest()
    {
        try
        {
            _faker.Create<int>();
            _faker.Create<string>();
            _faker.Create<List<int>>();
            _faker.Create<User>();
            _faker.Create<ClassA>();
        }
        catch (Exception ex)
        {
            Assert.Fail($"Возникло исключение: {ex.Message}");
        }
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

public class Product
{
    public string? Name { get; set; }
    public decimal Price { get; set; }
    public Category? Category { get; set; }
}

public class Category
{
    public string? Name { get; set; }
    public string? Description { get; set; }
}

public class MultipleConstructorsClass
{
    public string ConstructorUsed { get; }
    
    public MultipleConstructorsClass()
    {
        ConstructorUsed = "Default";
    }
    
    public MultipleConstructorsClass(int param1) : this()
    {
        ConstructorUsed = "OneParam";
    }
    
    public MultipleConstructorsClass(int param1, string param2) : this()
    {
        ConstructorUsed = "TwoParams";
    }
    
    public MultipleConstructorsClass(int param1, string param2, DateTime param3) : this()
    {
        ConstructorUsed = "ThreeParams";
    }
}

public struct PointStruct
{
    public int X { get; set; }
    public int Y { get; set; }
}

public struct RectangleStruct
{
    public int Width { get; }
    public int Height { get; }
    
    public RectangleStruct(int width, int height)
    {
        Width = width;
        Height = height;
    }
}

public class ClassA
{
    public string? Value { get; set; }
    public ClassB? B { get; set; }
}

public class ClassB
{
    public string? Value { get; set; }
    public ClassC? C { get; set; }
}

public class ClassC
{
    public string? Value { get; set; }
    public ClassA? A { get; set; }
}

public class CircularRoot
{
    public CircularChild? Child { get; set; }
}

public class CircularChild
{
    public CircularRoot? Root { get; set; }
    public CircularGrandchild? Grandchild { get; set; }
}

public class CircularGrandchild
{
    public CircularChild? Child { get; set; }
    public CircularRoot? Root { get; set; }
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

public class TestCityGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context) => "TestCity";
    public bool CanGenerate(Type type) => type == typeof(string);
}

public class TestAgeGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context) => 25;
    public bool CanGenerate(Type type) => type == typeof(int);
}

public class TestEmailGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context) => "test@test.com";
    public bool CanGenerate(Type type) => type == typeof(string);
}

public class TestPastDateGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context) => DateTime.Now.AddDays(-1);
    public bool CanGenerate(Type type) => type == typeof(DateTime);
}