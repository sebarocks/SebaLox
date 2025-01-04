using System.Text;

namespace LoxNet;

class AstPrinter : Expr.Visitor<string>
{
    public string Print(Expr expr)
    {
        return expr.Accept(this);
    }

    public string VisitBinaryExpr(Expr.Binary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Left, expr.Right);
    }

    public string VisitGroupingExpr(Expr.Grouping expr)
    {
        return Parenthesize("group", expr.Expression);
    }

    public string VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value.ToString() ?? "nil";
    }

    public string VisitUnaryExpr(Expr.Unary expr)
    {
        return Parenthesize(expr.Operator.Lexeme, expr.Right);
    }


    private string Parenthesize(string name, params Expr[] exprs)
    {
        StringBuilder sb = new();

        sb.Append('(').Append(name);
        foreach (Expr expr in exprs)
        {
            sb.Append(' ');
            sb.Append(expr.Accept(this));
        }
        sb.Append(')');

        return sb.ToString();
    }

    public static void ASTPrint(string[] args)
    {
        Expr expression = new Expr.Binary(
            new Expr.Unary(
                new Token(TokenType.MINUS, "-", null, 1),
                new Expr.Literal(123)
            ),
            new Token(TokenType.STAR, "*", null, 1),
            new Expr.Grouping(
                new Expr.Literal(45.67)
            )
        );

        Console.WriteLine( new AstPrinter().Print(expression) );
    }

}
