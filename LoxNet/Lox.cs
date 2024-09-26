using System.Text;

namespace LoxNet;

public class Lox
{
    private static bool HadError = false;

    public static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();
            if (line == null) break;
            Run(line);
            HadError = false;
        }
    }

    public static void RunFile(string path)
    {
        byte[] bytes = File.ReadAllBytes(path);
        string fileContent = Encoding.Default.GetString(bytes);
        Run(fileContent);
        if (HadError) Environment.Exit(65);
    }

    public static void Run(string source)
    {
        Scanner scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();

        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }
    }

    public static void Error(int line, string message)
    {
        Report(line, "", message);
    }

    public static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error {1}: {message}");
        HadError = true;
    }


}