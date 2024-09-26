using System.Reflection.Metadata.Ecma335;

namespace LoxNet;

public class Scanner
{

    private readonly string source;
    private readonly List<Token> tokens;

    public readonly Dictionary<string, TokenType> keywords = new Dictionary<string, TokenType>()
    {
        { "and",    TokenType.AND },
        { "class",  TokenType.CLASS },
        { "else",   TokenType.ELSE },
        { "false",  TokenType.FALSE },
        { "for",    TokenType.FOR },
        { "fun" ,   TokenType.FUN },
        { "if",     TokenType.IF },
        { "nil",    TokenType.NIL },
        { "or",     TokenType.OR },
        { "print",  TokenType.PRINT },
        { "return", TokenType.RETURN },
        { "super",  TokenType.SUPER },
        { "this",   TokenType.THIS },
        { "true",   TokenType.TRUE },
        { "var",    TokenType.VAR },
        { "while",  TokenType.WHILE }
    };

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source)
    {
        this.source = source;
        this.tokens = [];
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        tokens.Add(new Token(TokenType.EOF, "", null, line));
        return tokens;
    }

    private void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case ',': AddToken(TokenType.COMMA); break;
            case '.': AddToken(TokenType.DOT); break;
            case '-': AddToken(TokenType.MINUS); break;
            case '+': AddToken(TokenType.PLUS); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '*': AddToken(TokenType.STAR); break;

            case '!':
                AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
            case '=':
                AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
            case '<':
                AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;
            case '>':
                AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;

            case '/':
                if (Match('/'))
                {
                    while (Peek() != '\n' && !IsAtEnd()) Advance();
                }
                else
                {
                    AddToken(TokenType.SLASH);
                }
                break;

            // Ignore whitespace
            case ' ':
            case '\r':
            case '\t':
                break;

            // Line break
            case '\n': line++; break;

            case '"': StringLiteral(); break;

            default:
                if (IsDigit(c)) { NumberLiteral(); } 
                else if (IsAlpha(c)) { Identifier(); }
                else { Lox.Error(line, "Unexpected character."); }
                break;
        }
    }

    private void Identifier()
    {
        while (IsAlphaNumeric(Peek()) ) Advance();

        string text = source[start..current];

        if (keywords.TryGetValue(text, out TokenType type))
        {
            AddToken(type);
        }
        else
        {
            AddToken(TokenType.IDENTIFIER);
        }
    }

    private static bool IsAlphaNumeric(char c)
    {
        return IsAlpha(c) || IsDigit(c);
    }

    private static bool IsAlpha(char c)
    {
        bool result = (c >= 'A' && c <= 'Z') || (c >= 'a' && c <= 'z') || c == '_';
        return result;
    }

    private static bool IsDigit(char c)
    {
        return c >= '0' && c <= '9';
    }

    private void NumberLiteral()
    {
        while (IsDigit(Peek())) Advance();

        if (Peek() == '.' && IsDigit(PeekNext()))
        {
            Advance();

            while (IsDigit(Peek())) Advance();
        }

        AddToken(TokenType.NUMBER, Double.Parse(source[start..current]));
    }

    private void StringLiteral()
    {
        while(Peek() != '"' && !IsAtEnd())
        {
            if(Peek() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Lox.Error(line, "Unterminated string");
            return;
        }

        // Peek() == '"'
        Advance();
        string value = source[(start + 1) .. (current - 1)];
        AddToken(TokenType.STRING, value);
    }

    private char PeekNext()
    {
        if(current + 1 >= source.Length) return '\0';
        return source[current + 1];
    }

    private char Peek()
    {
        if (IsAtEnd()) return '\0';
        return source[current];
    }

    private bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (source[current] != expected) return false;
        current++;
        return true;
    }

    private char Advance()
    {
        current++;
        return source[current - 1];
    }

    private void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    private void AddToken(TokenType type, object? literal)
    {
        string text = source[start..current];
        tokens.Add(new Token(type, text, literal, line));
    }

    private bool IsAtEnd()
    {
        return current >= source.Length;
    }
}