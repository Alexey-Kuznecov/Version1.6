
using UnityCommander.CLI.Infrastructure;

[Obsolete("This class is for testing purposes only and should not be used in production.")]
public class ConsoleOutput //: IConsoleOutput
{
    public event Action<string>? TextWritten;
    public event Action? Cleared;

    public void Write(string message)
    {
        Console.Write(message);
    }
    public void WriteLine(string message)
    {
        Console.WriteLine(message);
    }

    public void WriteError(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }
    public void WriteWarning(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkYellow;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public void WriteSuccess(string message)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.DarkGreen;
        Console.WriteLine(message);
        Console.ForegroundColor = originalColor;
    }

    public void Clear()
    {
        Console.Clear();
    }

    public IConsoleActivity StartActivity(string message)
    {
        throw new NotImplementedException();
    }
}