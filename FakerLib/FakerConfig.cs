using System.Linq.Expressions;

namespace FakerLib;

public class FakerConfig
{
    private readonly List<CustomGeneratorInfo> _customGenerators = new();

    public void Add<TClass, TProperty, TGenerator>(Expression<Func<TClass, TProperty>> expression)
        where TGenerator : IValueGenerator, new()
    {
        _customGenerators.Add(new CustomGeneratorInfo
        {
            ClassType = typeof(TClass),
            Expression = expression,
            Generator = new TGenerator()
        });
    }

    public IEnumerable<CustomGeneratorInfo> GetCustomGenerators() => _customGenerators;

    public class CustomGeneratorInfo
    {
        public Type ClassType { get; set; } = null!;
        public LambdaExpression Expression { get; set; } = null!;
        public IValueGenerator Generator { get; set; } = null!;
    }
}