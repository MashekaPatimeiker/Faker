using System.Reflection;

namespace FakerLib;

public class ObjectCreator
{
    private readonly GeneratorRegistry _registry;
    private readonly Random _random;
    private readonly ExtendedGeneratorRegistry? _extendedRegistry;
    private readonly HashSet<int> _objectsInCreation = new();

    public ObjectCreator(GeneratorRegistry registry, Random random)
    {
        _registry = registry;
        _random = random;
        _extendedRegistry = registry as ExtendedGeneratorRegistry;
    }

    public object CreateObject(Type type, IFaker faker, int recursionDepth = 0)
    {
        if (recursionDepth > 10)
            return GetDefaultValue(type);

        var typeHash = type.GetHashCode() + recursionDepth;
        if (_objectsInCreation.Contains(typeHash))
            return GetDefaultValue(type);

        _objectsInCreation.Add(typeHash);

        try
        {
            if (IsSimpleType(type))
            {
                var context = new GeneratorContext(_random, faker, type);
                var result = _registry.GenerateValue(type, context);
                _objectsInCreation.Remove(typeHash);
                return result;
            }

            object? obj = CreateInstanceWithCustomGenerators(type, faker, recursionDepth);
            
            if (obj == null)
            {
                try
                {
                    obj = Activator.CreateInstance(type, true);
                }
                catch
                {
                    _objectsInCreation.Remove(typeHash);
                    return GetDefaultValue(type);
                }
            }

            if (obj != null)
            {
                FillPropertiesWithCustomGenerators(obj, type, faker);
            }

            _objectsInCreation.Remove(typeHash);
            return obj ?? GetDefaultValue(type);
        }
        catch
        {
            _objectsInCreation.Remove(typeHash);
            return GetDefaultValue(type);
        }
    }

    private object? CreateInstanceWithCustomGenerators(Type type, IFaker faker, int recursionDepth)
    {
        var constructors = type.GetConstructors(BindingFlags.Public | BindingFlags.Instance)
            .OrderByDescending(c => c.GetParameters().Length)
            .ToArray();

        foreach (var constructor in constructors)
        {
            try
            {
                var parameters = constructor.GetParameters();
                var parameterValues = new object[parameters.Length];

                for (int i = 0; i < parameters.Length; i++)
                {
                    var param = parameters[i];
                    
                    object? customValue = TryGetCustomGeneratorValue(type, param, faker);
                    
                    if (customValue != null)
                    {
                        parameterValues[i] = customValue;
                    }
                    else
                    {
                        parameterValues[i] = faker.Create(param.ParameterType);
                    }
                }

                return constructor.Invoke(parameterValues);
            }
            catch
            {
                continue;
            }
        }

        return null;
    }

    private object? TryGetCustomGeneratorValue(Type type, ParameterInfo param, IFaker faker)
    {
        if (_extendedRegistry == null || param.Name == null)
            return null;

        var directMatch = _extendedRegistry.GenerateForMember(type, param.Name, 
            new GeneratorContext(_random, faker, param.ParameterType));
    
        if (directMatch != null)
            return directMatch;

        var pascalName = char.ToUpperInvariant(param.Name[0]) + param.Name.Substring(1);
        var pascalMatch = _extendedRegistry.GenerateForMember(type, pascalName, 
            new GeneratorContext(_random, faker, param.ParameterType));
    
        if (pascalMatch != null)
            return pascalMatch;

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        foreach (var prop in properties)
        {
            if (string.Equals(prop.Name, param.Name, StringComparison.OrdinalIgnoreCase))
            {
                var propMatch = _extendedRegistry.GenerateForMember(type, prop.Name, 
                    new GeneratorContext(_random, faker, param.ParameterType));
            
                if (propMatch != null)
                    return propMatch;
            }
        }

        return null;
    }

    private void FillPropertiesWithCustomGenerators(object obj, Type type, IFaker faker)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (field.IsInitOnly || field.IsLiteral) continue;

            try
            {
                object? value = null;
                
                if (_extendedRegistry != null)
                {
                    value = _extendedRegistry.GenerateForMember(type, field.Name, 
                        new GeneratorContext(_random, faker, field.FieldType));
                }
                
                if (value == null)
                {
                    value = faker.Create(field.FieldType);
                }
                
                field.SetValue(obj, value);
            }
            catch { }
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true);

        foreach (var property in properties)
        {
            try
            {
                object? value = null;
                
                if (_extendedRegistry != null)
                {
                    value = _extendedRegistry.GenerateForMember(type, property.Name, 
                        new GeneratorContext(_random, faker, property.PropertyType));
                }
                
                if (value == null)
                {
                    value = faker.Create(property.PropertyType);
                }
                
                property.SetValue(obj, value);
            }
            catch { }
        }
    }

    private bool IsSimpleType(Type type)
    {
        return type.IsPrimitive || 
               type == typeof(string) || 
               type == typeof(DateTime) || 
               type == typeof(Guid) ||
               type == typeof(decimal) ||
               type.IsEnum;
    }

    private object GetDefaultValue(Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type)!;
        return null!;
    }
}