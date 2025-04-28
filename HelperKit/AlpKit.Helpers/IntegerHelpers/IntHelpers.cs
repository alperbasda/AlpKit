namespace AlpKit.Helpers.IntegerHelpers;

public class IntHelpers
{
    /// <summary>
    /// Verilen uzunlukta rasgele integer sayı üretir.
    /// </summary>
    /// <param name="length"></param>
    /// <returns></returns>
    public static int CreateNumber(int length)
    {
        int[] list = new int[length];
        Random rnd = new Random();
        for (int i = 0; i < length; i++)
            list[i] = rnd.Next(0, 9);

        return Convert.ToInt32(string.Join("", list));
    }
}
