using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace AlpKit.Helpers.StringHelpers;

public static class StrHelper
{
    /// <summary>
    /// Verilen string ifade boşluk içeriyorsa true döner
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool ContainSpace(string text)
    {
        return text.Contains(' ');
    }

    /// <summary>
    /// Verilen string ifade boşluk içermiyorsa true döner
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool NotContainSpace(string text)
    {
        return !text.Contains(' ');
    }

    /// <summary>
    /// Verilen string ifade geçerli bir mail adresi ise true döner
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsValidMailAddress(string text)
    {
        Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
        return regex.Match(text).Success;
    }

    /// <summary>
    /// Verilen string ifade geçerli bir telefon numarası ise true döner
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsValidPhoneNumber(string text)
    {
        if (Regex.IsMatch(text, "^[0-9]*$"))
        {
            if (text.Length == 10 || text.Length == 11)
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Verilen string ifade geçerli bir Tc kimlik numarası ise true döner
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static bool IsValidTcNumber(string tcNumber)
    {
        bool returnvalue = false;
        if (tcNumber.Length == 11)
        {
            Int64 ATCNO, BTCNO, TcNo;
            long C1, C2, C3, C4, C5, C6, C7, C8, C9, Q1, Q2;

            TcNo = Int64.Parse(tcNumber);

            ATCNO = TcNo / 100;
            BTCNO = TcNo / 100;

            C1 = ATCNO % 10; ATCNO = ATCNO / 10;
            C2 = ATCNO % 10; ATCNO = ATCNO / 10;
            C3 = ATCNO % 10; ATCNO = ATCNO / 10;
            C4 = ATCNO % 10; ATCNO = ATCNO / 10;
            C5 = ATCNO % 10; ATCNO = ATCNO / 10;
            C6 = ATCNO % 10; ATCNO = ATCNO / 10;
            C7 = ATCNO % 10; ATCNO = ATCNO / 10;
            C8 = ATCNO % 10; ATCNO = ATCNO / 10;
            C9 = ATCNO % 10; ATCNO = ATCNO / 10;
            Q1 = ((10 - ((((C1 + C3 + C5 + C7 + C9) * 3) + (C2 + C4 + C6 + C8)) % 10)) % 10);
            Q2 = ((10 - (((((C2 + C4 + C6 + C8) + Q1) * 3) + (C1 + C3 + C5 + C7 + C9)) % 10)) % 10);

            returnvalue = ((BTCNO * 100) + (Q1 * 10) + Q2 == TcNo);
        }
        return returnvalue; 
    }

    /// <summary>
    /// Verilen string ifadeyi geçerli bir url e çevirir. Türkçe karakter ve boşlukları düzenler.
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string ToUrl(this string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        text = text.ToLowerInvariant();

        var replacements = new Dictionary<char, char>
        {
            ['ç'] = 'c',
            ['ğ'] = 'g',
            ['ı'] = 'i',
            ['ö'] = 'o',
            ['ş'] = 's',
            ['ü'] = 'u'
        };
        var sb = new StringBuilder();
        foreach (var ch in text)
        {
            sb.Append(replacements.ContainsKey(ch) ? replacements[ch] : ch);
        }

        text = sb.ToString();

        text = text.Normalize(NormalizationForm.FormD);
        var chars = text
            .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
            .ToArray();
        text = new string(chars).Normalize(NormalizationForm.FormC);

        text = Regex.Replace(text, @"[^a-z0-9\s-]", "");
        text = Regex.Replace(text, @"\s+", "-");        
        text = Regex.Replace(text, @"-+", "-");         
        text = text.Trim('-');                          

        return text;
    }
}
