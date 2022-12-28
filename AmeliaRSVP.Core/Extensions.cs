namespace AmeliaRSVP.Core;

public static class Extensions
{
    public static bool IsNullOrEmpty(this string s) => string.IsNullOrEmpty(s);
    public static bool HasValue(this string s) => !s.IsNullOrEmpty();

    public static string FirstName(this string s)
    {
        if (s.Contains(" y "))
        {
            return s;
        }
        
        var parts = s.Split(" ");
        if (parts.Length > 1)
        {
            parts = parts.Take(parts.Length - 1).ToArray();
        }

        return string.Join(" ", parts);
    }
}
