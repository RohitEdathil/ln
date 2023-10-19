class Literal : Expression
{
    public object? Value;

    public Literal(object? value)
    {
        Value = value;
    }

}

class Grouping : Expression
{
    public Expression Expression;

    public Grouping(Expression expression)
    {
        Expression = expression;
    }

}

class Unary : Expression
{
    public Token Operator;
    public Expression Right;

    public Unary(Token op, Expression right)
    {
        Operator = op;
        Right = right;
    }

}

class Binary : Expression
{
    public Expression Left;
    public Token Operator;
    public Expression Right;

    public Binary(Expression left, Token op, Expression right)
    {
        Left = left;
        Operator = op;
        Right = right;
    }

}