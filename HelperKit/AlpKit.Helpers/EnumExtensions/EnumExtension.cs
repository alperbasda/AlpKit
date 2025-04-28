using System.ComponentModel;
using System.Reflection;

namespace AlpKit.Helpers.EnumExtensions;

public static class EnumExtension
{
    /// <summary>
    /// Enum üzerindeki description attributeun değerini döner.
    /// Değer yok ise enumun kendisini döner.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="enumerationValue"></param>
    /// <returns></returns>
    public static string GetDescription<T>(this T enumerationValue)
       where T : struct
    {
        Type type = enumerationValue.GetType();
        if (!type.IsEnum)
        {
            return "";
        }

        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString() ?? "");
        if (memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs.Length > 0)
            {
                //Pull out the description value
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue.ToString() ?? "Unknown";
    }

    /// <summary>
    /// Enum un Description attribute ünü döndürür
    /// </summary>
    /// <param name="type"></param>
    /// <param name="enumerationValue"></param>
    /// <returns></returns>
    public static string GetDescription(Type type, string enumerationValue)
    {
        if (!type.IsEnum)
        {
            return "";
        }
        //Tries to find a DescriptionAttribute for a potential friendly name
        //for the enum
        MemberInfo[] memberInfo = type.GetMember(enumerationValue.ToString());
        if (memberInfo.Length > 0)
        {
            object[] attrs = memberInfo[0].GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attrs.Length > 0)
            {
                //Pull out the description value
                return ((DescriptionAttribute)attrs[0]).Description;
            }
        }
        //If we have no description attribute, just return the ToString of the enum
        return enumerationValue;
    }

    /// <summary>
    /// Generic olarak geçilen tipin enum değerini verir
    /// </summary>
    /// <param></param>
    /// <param name="value"></param>
    /// <returns></returns>
    public static T ToEnum<T>(this string value)
    {
        return (T)Enum.Parse(typeof(T), value, true);
    }

    /// <summary>
    /// Enumun bir sonraki değerini verir. Eğer sonraki değer yok ise ilk değeri döndürür.
    /// </summary>
    /// <param></param>
    /// <param name="src"></param>
    /// <returns></returns>
    public static T Next<T>(this T src) where T : struct
    {
        if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf(arr, src) + 1;
        return arr.Length == j ? arr[0] : arr[j];
    }

    /// <summary>
    /// Enumun bir önceki değerini verir. Eğer önceki değer yok ise ilk değeri döndürür.
    /// </summary>
    /// <param></param>
    /// <param name="src"></param>
    /// <returns></returns>
    public static T Previous<T>(this T src) where T : struct
    {

        if (!typeof(T).IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", typeof(T).FullName));

        T[] arr = (T[])Enum.GetValues(src.GetType());
        int j = Array.IndexOf(arr, src) - 1;
        return j < 0 ? arr[0] : arr[j];
    }

    /// <summary>
    /// Verilen Enum tipinin kaç değer içerdiğini döndürür.
    /// </summary>
    /// <param></param>
    /// <param name="type"></param>
    /// <returns></returns>
    public static int EnumLength(this Type type)
    {
        if (!type.IsEnum) throw new ArgumentException(string.Format("Argument {0} is not an Enum", type.FullName));

        return Enum.GetNames(type).Length;
    }
}
