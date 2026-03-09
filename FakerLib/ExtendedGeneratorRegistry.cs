using System.Linq.Expressions;

namespace FakerLib;

public class ExtendedGeneratorRegistry : GeneratorRegistry
{
    private readonly Dictionary<string, IValueGenerator> _customGenerators = new();

    public ExtendedGeneratorRegistry(FakerConfig config)
    {
        foreach (var customGenerator in config.GetCustomGenerators())
        {
            if (customGenerator.Expression.Body is MemberExpression memberExpression)
            {
                var key = $"{customGenerator.ClassType.FullName}.{memberExpression.Member.Name}";
                _customGenerators[key] = customGenerator.Generator;
            }
        }
    }

    public bool HasCustomGenerator(Type classType, string memberName)
    {
        if (classType.FullName == null || memberName == null) return false;
        var key = $"{classType.FullName}.{memberName}";
        return _customGenerators.ContainsKey(key);
    }

    public object? GenerateForMember(Type classType, string memberName, GeneratorContext context)
    {
        if (classType.FullName == null || memberName == null) return null;
        
        var key = $"{classType.FullName}.{memberName}";
        if (_customGenerators.TryGetValue(key, out var generator))
        {
            return generator.Generate(context.TargetType, context);
        }
        
        return null;
    }
}