namespace LoxNet;

public abstract class Expr
{
    public interface Visitor<R>
    {
        R VisitBinaryExpr(Binary expr);
        R VisitGroupingExpr(Grouping expr);
        R VisitLiteralExpr(Literal expr);
        R VisitUnaryExpr(Unary expr);
    }
    public class Binary : Expr
    {
        public Binary(Expr left, Token @operator, Expr right)
        {
            this.Left = left;
            this.Operator = @operator;
            this.Right = right;
        }
        
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitBinaryExpr(this);
        }
        
        public readonly Expr Left;
        public readonly Token Operator;
        public readonly Expr Right;
     }
    public class Grouping : Expr
    {
        public Grouping(Expr expression)
        {
            this.Expression = expression;
        }
        
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitGroupingExpr(this);
        }
        
        public readonly Expr Expression;
     }
    public class Literal : Expr
    {
        public Literal(Object? value)
        {
            this.Value = value;
        }
        
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitLiteralExpr(this);
        }
        
        public readonly Object? Value;
     }
    public class Unary : Expr
    {
        public Unary(Token @operator, Expr right)
        {
            this.Operator = @operator;
            this.Right = right;
        }
        
        public override R Accept<R>(Visitor<R> visitor)
        {
            return visitor.VisitUnaryExpr(this);
        }
        
        public readonly Token Operator;
        public readonly Expr Right;
     }
    
    public abstract R Accept<R>(Visitor<R> visitor);
}
