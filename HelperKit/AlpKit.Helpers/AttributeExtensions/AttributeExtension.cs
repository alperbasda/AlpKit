using System.Reflection;

namespace AlpKit.Helpers.AttributeExtensions;

public static class AttributeExtension
{
    public static string GetAttributeValue<T, TClass>(this TClass obj, string propertyName)
    where T : Attribute
    where TClass : class, new()
    {
        try
        {
            var attribute = obj.GetType().GetCustomAttribute(typeof(T), false)!;
            var attrProperty = attribute.GetType().GetProperty(propertyName)!;
            return attrProperty.GetValue(attribute)!.ToString()!;
        }
        catch (Exception ex)
        {
            throw new Exception($"AttributeHelper --> GetAttributeValue({obj.GetType().Name},{propertyName}) {ex.Message}");

        }
    }

}
