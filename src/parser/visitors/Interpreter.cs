class Interpreter : IExpressionVisitor<object?>
{

    ErrorHandler errorHandler;

    public Interpreter(ErrorHandler errorHandler)
    {
        this.errorHandler = errorHandler;
    }

    class RuntimeError : Exception
    {
        public Token Token { get; }
        public RuntimeError(Token token, string message) : base(message)
        {
            Token = token;
        }
    }

    public string? Interpret(Expression? expression)
    {
        try
        {
            return Stringify(expression?.Accept(this));
        }
        catch (RuntimeError error)
        {
            errorHandler.Error(error.Token.Line, error.Message);
            return null;
        }

    }

    public object? Visit(Literal expression)
    {
        return expression.Value;
    }

    public object? Visit(Grouping expression)
    {
        return expression.Expression.Accept(this);
    }

    public object? Visit(Unary expression)
    {
        var right = expression.Right.Accept(this);

        switch (expression.Operator.Type)
        {
            case TokenType.MINUS:
                Ensure(right is double, expression.Operator, "Operand must be a number.");
                Ensure(right != null, expression.Operator, "Operand must not be null.");
                return -(double)right!;
            case TokenType.BANG:
                return !IsTruthy(right);
        }

        return null;
    }

    public object? Visit(Binary expression)
    {
        var left = expression.Left.Accept(this);
        var right = expression.Right.Accept(this);

        switch (expression.Operator.Type)
        {
            // Arithmetic
            case TokenType.MINUS:
                EnsureAreNumbers(left, right, expression.Operator);
                return (double)left! - (double)right!;
            case TokenType.SLASH:
                EnsureAreNumbers(left, right, expression.Operator);
                Ensure((double)right! != 0, expression.Operator, "Division by zero.");
                return (double)left! / (double)right!;
            case TokenType.STAR:
                EnsureAreNumbers(left, right, expression.Operator);
                return (double)left! * (double)right!;
            case TokenType.PLUS:
                if (left is double && right is double)
                {
                    return (double)left! + (double)right!;
                }
                if (left is string && right is string)
                {
                    return (string)left! + (string)right!;
                }
                throw new RuntimeError(expression.Operator, "Operands must be two numbers or two strings.");

            // Comparison
            case TokenType.GREATER:
                EnsureAreNumbers(left, right, expression.Operator);
                return (double)left! > (double)right!;
            case TokenType.GREATER_EQUAL:
                EnsureAreNumbers(left, right, expression.Operator);
                return (double)left! >= (double)right!;
            case TokenType.LESS:
                EnsureAreNumbers(left, right, expression.Operator);
                return (double)left! < (double)right!;
            case TokenType.LESS_EQUAL:
                EnsureAreNumbers(left, right, expression.Operator);
                return (double)left! <= (double)right!;

            // Equality
            case TokenType.BANG_EQUAL:
                return !IsEqual(left, right);
            case TokenType.EQUAL_EQUAL:
                return IsEqual(left, right);
        }
        return null;
    }


    // Helpers
    static void Ensure(bool condition, Token token, string message)
    {
        if (!condition)
        {
            throw new RuntimeError(token, message);
        }
    }

    static void EnsureAreNumbers(object? left, object? right, Token token)
    {
        Ensure(left is double && right is double, token, "Operands must be numbers.");
    }

    static string Stringify(object? value)
    {
        if (value == null) return "nil";
        if (value is double d)
        {
            var text = d.ToString();
            if (text.EndsWith(".0"))
            {
                text = text[..^2];
            }
            return text;
        }
        return value.ToString()!;
    }


    static bool IsTruthy(object? value)
    {
        if (value == null) return false;
        if (value is bool v) return v;
        if (value is double d) return d != 0;
        return true;
    }

    static bool IsEqual(object? a, object? b)
    {
        if (a == null && b == null) return true;
        if (a == null) return false;
        return a.Equals(b);
    }
}