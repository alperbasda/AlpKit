namespace AlpKit.Helpers.RegexExtensions;

public static class RegexExtension
{
    public static bool IsRegex(string input)
    {
        try
        {
            if (!input.StartsWith('^') || !input.EndsWith('$'))
            {
                return false;
            }
            System.Text.RegularExpressions.Regex regex = new System.Text.RegularExpressions.Regex(input);
            return true;
        }
        catch (ArgumentException)
        {
            return false;
        }
    }

    public static bool IsMatchWithRegex(string input, string regexPattern)
    {
        if (!IsRegex(regexPattern))
        {
            return false;
        }
        return System.Text.RegularExpressions.Regex.IsMatch(input, regexPattern);
    }
}
