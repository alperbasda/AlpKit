using System.Security.Cryptography;
using System.Text;
namespace AlpKit.Helpers.HashingHelpers;

public class EncryptionService
{
    private Aes GetCryptoProvider(string securityKey)
    {
        using var sha256 = SHA256.Create();
        var key = sha256.ComputeHash(Encoding.UTF8.GetBytes(securityKey));

        var aes = Aes.Create();
        aes.Key = key;
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        // IV (Initialization Vector) oluşturulur
        aes.GenerateIV();
        return aes;
    }

    public string Encrypt(string plainString, string securityKey)
    {
        var data = Encoding.UTF8.GetBytes(plainString);

        // AES sağlayıcısını oluştur
        using var aes = GetCryptoProvider(securityKey);

        // Şifreleme işlemi
        using var transform = aes.CreateEncryptor();
        var encryptedData = transform.TransformFinalBlock(data, 0, data.Length);

        // IV'yi şifreli veriye ekle ve Base64 olarak döndür
        var result = new byte[aes.IV.Length + encryptedData.Length];
        Array.Copy(aes.IV, 0, result, 0, aes.IV.Length);
        Array.Copy(encryptedData, 0, result, aes.IV.Length, encryptedData.Length);

        return Convert.ToBase64String(result);
    }

    public string Decrypt(string encryptedString, string securityKey)
    {
        var encryptedDataWithIv = Convert.FromBase64String(encryptedString);

        // AES sağlayıcısını oluştur
        using var aes = GetCryptoProvider(securityKey);

        // IV'yi ve şifreli veriyi ayır
        var iv = new byte[aes.BlockSize / 8];
        var encryptedData = new byte[encryptedDataWithIv.Length - iv.Length];

        Array.Copy(encryptedDataWithIv, 0, iv, 0, iv.Length);
        Array.Copy(encryptedDataWithIv, iv.Length, encryptedData, 0, encryptedData.Length);

        aes.IV = iv;

        // Decrypt işlemi
        using var transform = aes.CreateDecryptor();
        var decryptedData = transform.TransformFinalBlock(encryptedData, 0, encryptedData.Length);

        return Encoding.UTF8.GetString(decryptedData);
    }
}
