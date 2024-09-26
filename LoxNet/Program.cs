using System.Text;

namespace LoxNet;

public class Program
{
    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: loxnet [script]");
            Environment.Exit(64);
        }
        else if (args.Length == 1)
        {
            Lox.RunFile(args[0]);
        }
        else
        {
            Lox.RunPrompt();
        }
    }

}