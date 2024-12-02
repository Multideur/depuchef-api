using System.Reflection;

namespace DepuChef.Application.Utilities;

public static class TypeExtensions
{
    public static IEnumerable<T?> GetAllPublicConstantValues<T>(this Type type) => 
        type.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
            .Where(f => f.IsLiteral && !f.IsInitOnly && f.FieldType == typeof(T))
            .Select(f => (T?)f.GetRawConstantValue());
}
