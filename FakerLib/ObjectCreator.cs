using System.Reflection;

namespace FakerLib;

public class ObjectCreator
{
    private readonly GeneratorRegistry _registry;
    private readonly Random _random;
    private readonly HashSet<int> _objectsInCreation = new();

    public ObjectCreator(GeneratorRegistry registry, Random random)
    {
        _registry = registry;
        _random = random;
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
            if (_registry.CanGenerate(type))
            {
                var context = new GeneratorContext(_random, faker, type);
                var result = _registry.GenerateValue(type, context);
                _objectsInCreation.Remove(typeHash);
                return result;
            }

            object? obj = null;

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
                        parameterValues[i] = faker.Create(parameters[i].ParameterType);
                    }

                    obj = constructor.Invoke(parameterValues);
                    break;
                }
                catch
                {
                    continue;
                }
            }

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
                // Заполняем поля и свойства
                FillMembers(obj, type, faker);
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

    private void FillMembers(object obj, Type type, IFaker faker)
    {
        var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
        foreach (var field in fields)
        {
            if (!field.IsInitOnly && !field.IsLiteral)
            {
                try
                {
                    var value = faker.Create(field.FieldType);
                    field.SetValue(obj, value);
                }
                catch
                {
                    //gay
                }
            }
        }

        var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite && p.SetMethod?.IsPublic == true);

        foreach (var property in properties)
        {
            try
            {
                var value = faker.Create(property.PropertyType);
                property.SetValue(obj, value);
            }
            catch
            {
                // gay
            }
        }
    }

    private object GetDefaultValue(Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type)!;
        return null!;
    }
}