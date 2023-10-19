

abstract class Expression
{
    public T Accept<T>(IExpressionVisitor<T> visitor) => visitor.Visit((dynamic)this);

}

interface IExpressionVisitor<T>
{
    public T Visit(Literal expression);
    public T Visit(Grouping expression);
    public T Visit(Unary expression);
    public T Visit(Binary expression);
}