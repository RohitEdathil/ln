
class Token
{
    public readonly TokenType type;
    public readonly object? Literal;
    public readonly int Line;

    public Token(TokenType type, object? literal, int line)
    {
        this.type = type;
        this.Literal = literal;
        this.Line = line;
    }

    public override string ToString()
    {
        return type + " " + Literal;
    }
}

enum TokenType
{
    // Single-character tokens
    LEFT_PAREN, RIGHT_PAREN, LEFT_BRACE, RIGHT_BRACE,
    COMMA, DOT, MINUS, PLUS, SEMICOLON, SLASH, STAR,

    // One or two character tokens
    BANG, BANG_EQUAL,
    EQUAL, EQUAL_EQUAL,
    GREATER, GREATER_EQUAL,
    LESS, LESS_EQUAL,

    // Literals
    IDENTIFIER, STRING, NUMBER,

    // Keywords
    AND, CLASS, ELSE, FALSE, FUN, FOR, IF, NIL, OR,
    PRINT, RETURN, SUPER, THIS, TRUE, VAR, WHILE,

    // Misc
    EOF
}