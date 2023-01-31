namespace VBY.Basic;

public static class Utils
{
    public static void WriteColorLine(string value,ConsoleColor color = ConsoleColor.Red)
    {
        Console.ForegroundColor = color;
        Console.WriteLine(value);
        Console.ResetColor();
    }
    public static void WriteInfoLine(string value) => WriteColorLine(value, ConsoleColor.Yellow);
    public static void WriteSuccessLine(string value) => WriteColorLine(value, ConsoleColor.Green);
}
