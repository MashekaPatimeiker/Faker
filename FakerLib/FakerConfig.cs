using System.Linq.Expressions;

namespace FakerLib;

public class FakerConfig
{
    private readonly Dictionary<string, Func<GeneratorContext, object>> _customGenerators = new();

    public void Add<TClass, TProperty, TGenerator>(Expression<Func<TClass, TProperty>> expression)
        where TGenerator : IValueGenerator, new()
    {
        var memberExpression = expression.Body as MemberExpression;
        if (memberExpression == null)
            throw new ArgumentException("Expression must be a member access");

        var memberName = memberExpression.Member.Name;
        var generator = new TGenerator();
        
        _customGenerators[GetKey(typeof(TClass), memberName)] = context => 
            generator.Generate(typeof(TProperty), context);
    }

    public bool TryGetGenerator(Type classType, string memberName, out Func<GeneratorContext, object>? generator)
    {
        return _customGenerators.TryGetValue(GetKey(classType, memberName), out generator);
    }

    private string GetKey(Type classType, string memberName) => $"{classType.FullName}.{memberName}";
}

public class ExtendedGeneratorRegistry : GeneratorRegistry
{
    private readonly FakerConfig? _config;

    public ExtendedGeneratorRegistry(FakerConfig? config = null)
    {
        _config = config;
    }

    public bool HasCustomGeneratorForMember(Type classType, string memberName)
    {
        return _config != null && _config.TryGetGenerator(classType, memberName, out _);
    }

    public object? GenerateForMember(Type classType, string memberName, Type memberType, GeneratorContext context)
    {
        if (_config != null && _config.TryGetGenerator(classType, memberName, out var generator))
            return generator?.Invoke(context);

        return null;
    }
}