using System.Text;

namespace AlpKit.Helpers.CoderHelpers;

public class Coder
{
    public static string? DecodeFromBase64(string? base64EncodedData)
    {
        if (string.IsNullOrEmpty(base64EncodedData))
            return null;

        byte[] byteArray = Convert.FromBase64String(base64EncodedData);

        return Encoding.UTF8.GetString(byteArray);
    }

    public static string? EncodeToBase64(string? plainText)
    {
        if (string.IsNullOrEmpty(plainText))
            return null;

        byte[] byteArray = Encoding.UTF8.GetBytes(plainText);

        return Convert.ToBase64String(byteArray);
    }
}

