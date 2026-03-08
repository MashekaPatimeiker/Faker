namespace FakerLib.Generators;

public class IntGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
        => context.Random.Next();

    public bool CanGenerate(Type type) 
        => type == typeof(int);
}

public class LongGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var buffer = new byte[8];
        context.Random.NextBytes(buffer);
        return BitConverter.ToInt64(buffer, 0);
    }

    public bool CanGenerate(Type type) 
        => type == typeof(long);
}

public class DoubleGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
        => context.Random.NextDouble() * 1000;

    public bool CanGenerate(Type type) 
        => type == typeof(double);
}

public class FloatGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
        => (float)(context.Random.NextDouble() * 1000);

    public bool CanGenerate(Type type) 
        => type == typeof(float);
}

public class BoolGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
        => context.Random.Next(2) == 0;

    public bool CanGenerate(Type type) 
        => type == typeof(bool);
}

public class StringGenerator : IValueGenerator
{
    private const string Chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var length = context.Random.Next(5, 20);
        var result = new char[length];
        
        for (int i = 0; i < length; i++)
            result[i] = Chars[context.Random.Next(Chars.Length)];
        
        return new string(result);
    }

    public bool CanGenerate(Type type) 
        => type == typeof(string);
}

public class DateTimeGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var start = new DateTime(2000, 1, 1);
        var range = (DateTime.Now - start).Days;
        return start.AddDays(context.Random.Next(range));
    }

    public bool CanGenerate(Type type) 
        => type == typeof(DateTime);
}

public class GuidGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
        => Guid.NewGuid();

    public bool CanGenerate(Type type) 
        => type == typeof(Guid);
}

public class DecimalGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var scale = (byte)context.Random.Next(29);
        var sign = context.Random.Next(2) == 0;
        return new decimal(
            context.Random.Next(), 
            context.Random.Next(), 
            context.Random.Next(), 
            sign, 
            scale);
    }

    public bool CanGenerate(Type type) 
        => type == typeof(decimal);
}

public class CharGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
        => (char)context.Random.Next(65, 122);

    public bool CanGenerate(Type type) 
        => type == typeof(char);
}

public class ByteGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        var buffer = new byte[1];
        context.Random.NextBytes(buffer);
        return buffer[0];
    }

    public bool CanGenerate(Type type) 
        => type == typeof(byte);
}