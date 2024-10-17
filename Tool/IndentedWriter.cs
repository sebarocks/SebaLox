using System;
using System.IO;

public class IndentedWriter
{
    private readonly TextWriter _writer;
    private int _indentLevel;
    private readonly string _indentString;

    public IndentedWriter(TextWriter writer, int indentSize = 4)
    {
        _writer = writer;
        _indentString = new string(' ', indentSize);
    }

    public void IncreaseIndent() => _indentLevel++;

    public void DecreaseIndent() => _indentLevel = Math.Max(0, _indentLevel - 1);

    public void WriteLine(string line)
    {
        _writer.WriteLine(new string(' ', _indentLevel * _indentString.Length) + line);
    }

    public void WriteLine()
    {
        _writer.WriteLine(new string(' ', _indentLevel * _indentString.Length) );
    }
}

// Usage:
// var writer = new IndentedWriter(Console.Out);
// writer.WriteLine("Root level");
// writer.IncreaseIndent();
// writer.WriteLine("Indented once");
