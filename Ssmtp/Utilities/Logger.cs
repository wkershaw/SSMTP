namespace Ssmtp.Utilities;

internal static class Logger
{
    public static void LogDebug(string message)
    {
        Console.WriteLine($"[Debug] {message}");
    }

    public static void LogError(string message, Exception? ex = null)
    {
        Console.WriteLine($"[Error] {message}");
        if (ex is not null)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}