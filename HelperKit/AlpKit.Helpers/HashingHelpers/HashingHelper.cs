using System.Security.Cryptography;
using System.Text;

namespace AlpKit.Helpers.HashingHelpers;

public class HashingHelper
{
    public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using HMACSHA512 hMACSHA = new HMACSHA512();
        passwordSalt = hMACSHA.Key;
        passwordHash = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
    }

    public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {
        using (HMACSHA512 hMACSHA = new HMACSHA512(passwordSalt))
        {
            byte[] array = hMACSHA.ComputeHash(Encoding.UTF8.GetBytes(password));
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] != passwordHash[i])
                {
                    return false;
                }
            }
        }

        return true;
    }
}
