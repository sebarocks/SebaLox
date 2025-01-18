using System.Text;

namespace LoxNet;

public class Lox
{
    private static readonly Interpreter interpreter = new Interpreter();

    private static bool HadError = false;
    private static bool HadRuntimeError = false;

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
        if (HadRuntimeError) Environment.Exit(70);
    }

    public static void Run(string source)
    {
        Scanner scanner = new(source);
        List<Token> tokens = scanner.ScanTokens();
        Parser parser = new Parser(tokens);
        Expr? expression = parser.Parse();

        if (HadError || expression is null) return;

        interpreter.Interpret(expression);

        Console.WriteLine(new AstPrinter().Print(expression));
    }

    public static void RunScanner(string source)
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

    public static void Error(Token token, string message)
    {
        if(token.Type == TokenType.EOF)
        {
            Report(token.Line, " at end", message);
        } 
        else
        {
            Report(token.Line, $" at '{token.Lexeme}'", message);
        }
    }

    public static void runtimeError(RuntimeError error)
    {
        Console.WriteLine($"{error.Message} [ line {error.Token.Line}]");
        HadRuntimeError = true;
    }

    public static void Report(int line, string where, string message)
    {
        Console.WriteLine($"[line {line}] Error {where}: {message}");
        HadError = true;
    }


}