using System.Collections;

namespace FakerLib.Generators;

public class CollectionGenerator : IValueGenerator
{
    public object Generate(Type typeToGenerate, GeneratorContext context)
    {
        try
        {
            Type elementType;
            bool isArray = typeToGenerate.IsArray;
            
            if (isArray)
            {
                elementType = typeToGenerate.GetElementType()!;
            }
            else
            {
                if (!typeToGenerate.IsGenericType)
                    throw new ArgumentException("Type is not a generic collection");
                
                elementType = typeToGenerate.GetGenericArguments()[0];
            }

            var listType = typeof(List<>).MakeGenericType(elementType);
            var list = (IList)Activator.CreateInstance(listType)!;

            var count = context.Random.Next(1, 6); 
            for (int i = 0; i < count; i++)
            {
                var element = context.Faker.Create(elementType);
                list.Add(element);
            }

            if (isArray)
            {
                var array = Array.CreateInstance(elementType, count);
                for (int i = 0; i < count; i++)
                {
                    array.SetValue(list[i], i);
                }
                return array;
            }

            if (typeToGenerate != listType)
            {
                try
                {
                    var constructor = typeToGenerate.GetConstructor(new[] { typeof(IEnumerable<>).MakeGenericType(elementType) });
                    if (constructor != null)
                    {
                        return constructor.Invoke(new object[] { list });
                    }

                    var collection = Activator.CreateInstance(typeToGenerate);
                    var addMethod = typeToGenerate.GetMethod("Add", new[] { elementType });
                    
                    if (addMethod != null && collection != null)
                    {
                        foreach (var item in list)
                        {
                            addMethod.Invoke(collection, new[] { item });
                        }
                        return collection;
                    }
                }
                catch
                {
                    return list;
                }
            }

            return list;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in CollectionGenerator: {ex.Message}");
            var listType = typeof(List<>).MakeGenericType(typeof(object));
            return Activator.CreateInstance(listType)!;
        }
    }

    public bool CanGenerate(Type type)
    {
        if (type.IsArray)
            return true;

        if (type.IsGenericType)
        {
            var genericType = type.GetGenericTypeDefinition();
            return genericType == typeof(List<>) ||
                   genericType == typeof(IList<>) ||
                   genericType == typeof(ICollection<>) ||
                   genericType == typeof(IEnumerable<>);
        }

        return false;
    }
}