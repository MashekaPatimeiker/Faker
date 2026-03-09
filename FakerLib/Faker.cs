using FakerLib;

namespace FakerLib;

public class Faker : IFaker
{
    private readonly Random _random;
    private readonly GeneratorRegistry _registry;
    private readonly ObjectCreator _objectCreator;

    public Faker() : this(null)
    {
    }

    public Faker(FakerConfig? config)
    {
        _random = new Random();
        _registry = config != null 
            ? new ExtendedGeneratorRegistry(config) 
            : new GeneratorRegistry();
        _objectCreator = new ObjectCreator(_registry, _random);
    }

    public T Create<T>()
    {
        return (T)Create(typeof(T));
    }

    public object Create(Type type)
    {
        return _objectCreator.CreateObject(type, this);
    }

    internal object Create(Type type, int recursionDepth = 0)
    {
        return _objectCreator.CreateObject(type, this, recursionDepth);
    }
}