using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoxNet;

public class Interpreter : Expr.Visitor<object?>
{
    public object? VisitLiteralExpr(Expr.Literal expr)
    {
        return expr.Value;
    }

    public object? VisitGroupingExpr(Expr.Grouping expr)
    {
        return Evaluate(expr.Expression);
    }

    public object? VisitUnaryExpr(Expr.Unary expr)
    {
        object? right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.BANG:
                return !IsTruthy(right);
            case TokenType.MINUS:
                CheckNumberOperand(expr.Operator, right);
                return - (double?)(right);
        }

        return null;
    }

    public object? VisitBinaryExpr(Expr.Binary expr)
    {
        object? left = Evaluate(expr.Left);
        object? right = Evaluate(expr.Right);

        switch (expr.Operator.Type)
        {
            case TokenType.GREATER:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left > (double?)right;

            case TokenType.GREATER_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left >= (double?)right;

            case TokenType.LESS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left < (double?)right;

            case TokenType.LESS_EQUAL:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left <= (double?)right;

            case TokenType.MINUS:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left - (double?)right;

            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left + (double)right;
                }
                if (left is string && right is string)
                {
                    return (string)left + (string)right;
                }
                throw new RuntimeError(expr.Operator, "Operands must be two numbers or two strings.");

            case TokenType.SLASH:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left / (double?)right;

            case TokenType.STAR:
                CheckNumberOperands(expr.Operator, left, right);
                return (double?)left * (double?)right;

            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);

            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);

        }
        return null;
    }

    private void CheckNumberOperands(Token @operator, object? left, object? right)
    {
        if (left is double && right is double) return;
        throw new RuntimeError(@operator, "Operands must be numbers.");
    }

    private void CheckNumberOperand(Token @operator, object? operand)
    {
        if (operand is double) return;
        throw new RuntimeError(@operator, "Operand must be a number.");
    }

    private object? Evaluate(Expr expr)
    {
        return expr.Accept(this);
    }

    private bool IsTruthy(object? obj)
    {
        if (obj == null) return false;
        if (obj is bool) return (bool)obj;
        return true;
    }

    private bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }

    private string Stringify(object obj)
    {
        if (obj == null) return "nil";

        if (obj is double)
        {
            string text = obj.ToString() ?? "nil";

            if (text.EndsWith(".0"))
            {
                text = text.Substring(0, text.Length - 2);
            }

            return text;

        }

        return obj.ToString() ?? "nil";
    }

    public void Interpret(Expr expression)
    {
        try
        {
            object? value = Evaluate(expression);
            Console.WriteLine(Stringify(value!));
        } catch (RuntimeError error)
        {
            Lox.runtimeError(error);
        }
    }
}
