using FakerLib.Generators;

namespace FakerLib;

public class GeneratorRegistry
{
    private readonly Dictionary<Type, IValueGenerator> _generators = new();
    private readonly List<IValueGenerator> _baseGenerators = new();

    public GeneratorRegistry()
    {
        RegisterGenerator(new IntGenerator());
        RegisterGenerator(new LongGenerator());
        RegisterGenerator(new DoubleGenerator());
        RegisterGenerator(new FloatGenerator());
        RegisterGenerator(new BoolGenerator());
        RegisterGenerator(new StringGenerator());
        RegisterGenerator(new DateTimeGenerator());
        RegisterGenerator(new GuidGenerator());
        RegisterGenerator(new DecimalGenerator());
        RegisterGenerator(new CharGenerator());
        RegisterGenerator(new ByteGenerator());
        
        _baseGenerators.Add(new CollectionGenerator());
    }

    private void RegisterGenerator(IValueGenerator generator)
    {
        var types = AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => generator.CanGenerate(t));

        foreach (var type in types)
        {
            if (!_generators.ContainsKey(type))
            {
                _generators[type] = generator;
            }
        }
    }

    public bool CanGenerate(Type type)
    {
        if (_generators.ContainsKey(type))
            return true;

        return _baseGenerators.Any(g => g.CanGenerate(type));
    }

    public object GenerateValue(Type type, GeneratorContext context)
    {
        if (_generators.TryGetValue(type, out var generator))
        {
            return generator.Generate(type, context);
        }

        foreach (var gen in _baseGenerators)
        {
            if (gen.CanGenerate(type))
            {
                return gen.Generate(type, context);
            }
        }

        throw new InvalidOperationException($"No generator found for type {type}");
    }
}