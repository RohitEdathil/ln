
public class AstPrinter : IExpressionVisitor<string>
{
    string IExpressionVisitor<string>.Visit(Literal expression)
    => expression.Value?.ToString() ?? "nil";

    string IExpressionVisitor<string>.Visit(Grouping expression)
    => $"(group {expression.Expression.Accept(this)})";

    string IExpressionVisitor<string>.Visit(Unary expression)
    => $"({expression.Operator.type} {expression.Right.Accept(this)})";

    string IExpressionVisitor<string>.Visit(Binary expression)
    => $"({expression.Left.Accept(this)} {expression.Operator.type} {expression.Right.Accept(this)})";
}