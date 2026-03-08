namespace FakerLib;

public class GeneratorContext
{
    public Random Random { get; }
    public IFaker Faker { get; }
    public Type TargetType { get; }

    public GeneratorContext(Random random, IFaker faker, Type targetType)
    {
        Random = random;
        Faker = faker;
        TargetType = targetType;
    }
}
