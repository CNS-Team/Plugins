namespace Replenisher;

public static class Extensions
{
    public static string FirstCharToUpper(this string input)
    {
        return string.IsNullOrEmpty(input)
            ? throw new ArgumentException("String cannot be empty.")
            : input.First().ToString().ToUpper() + input[1..];
    }
}