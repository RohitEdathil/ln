
class Parser
{
    ErrorHandler errorHandler;

    class ParseError : Exception { }

    List<Token> Tokens;
    int Current = 0;

    public Parser(List<Token> tokens, ErrorHandler error)
    {
        Tokens = tokens;
        errorHandler = error;
    }

    public Expression? Parse()
    {
        try
        {
            return Expression();
        }
        catch (ParseError)
        {
            return null;
        }
    }

    Expression Expression()
    {
        return Equality();
    }

    Expression Equality()
    {
        var left = Comparison();

        while (MatchAny(TokenType.BANG_EQUAL, TokenType.EQUAL_EQUAL))
        {
            var op = PreviousToken;
            var right = Comparison();
            left = new Binary(left, op, right);
        }

        return left;
    }

    Expression Comparison()
    {
        var left = Term();

        while (MatchAny(TokenType.GREATER, TokenType.GREATER_EQUAL, TokenType.LESS, TokenType.LESS_EQUAL))
        {
            var op = PreviousToken;
            var right = Term();
            left = new Binary(left, op, right);
        }

        return left;
    }

    Expression Term()
    {
        var left = Factor();

        while (MatchAny(TokenType.MINUS, TokenType.PLUS))
        {
            var op = PreviousToken;
            var right = Factor();
            left = new Binary(left, op, right);
        }

        return left;
    }

    Expression Factor()
    {
        var left = Unary();

        while (MatchAny(TokenType.SLASH, TokenType.STAR))
        {
            var op = PreviousToken;
            var right = Unary();
            left = new Binary(left, op, right);
        }

        return left;
    }

    Expression Unary()
    {
        if (MatchAny(TokenType.BANG, TokenType.MINUS))
        {
            var op = PreviousToken;
            var right = Unary();
            return new Unary(op, right);
        }

        return Primary();
    }

    Expression Primary()
    {
        if (MatchAny(TokenType.FALSE)) return new Literal(false);
        if (MatchAny(TokenType.TRUE)) return new Literal(true);
        if (MatchAny(TokenType.NIL)) return new Literal(null);

        if (MatchAny(TokenType.NUMBER, TokenType.STRING))
        {
            return new Literal(PreviousToken.Literal!);
        }

        if (MatchAny(TokenType.LEFT_PAREN))
        {
            var expr = Expression();

            if (!MatchAny(TokenType.RIGHT_PAREN))
                throw Error(CurrentToken, "Expect ')' after expression.");

            return new Grouping(expr);
        }
        throw Error(CurrentToken, "Expect expression.");
    }

    void Synchronize()
    {
        Consume();

        while (!IsAtEnd)
        {
            if (PreviousToken.Type == TokenType.SEMICOLON) return;

            switch (CurrentToken.Type)
            {
                case TokenType.CLASS:
                case TokenType.FUN:
                case TokenType.VAR:
                case TokenType.FOR:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.PRINT:
                case TokenType.RETURN:
                    return;
            }

            Consume();
        }
    }

    ParseError Error(Token token, string message)
    {
        errorHandler.Error(token.Line, message);
        return new ParseError();
    }

    // Helpers
    Token CurrentToken => Tokens[Current];
    Token PreviousToken => Tokens[Current - 1];
    bool IsAtEnd => CurrentToken.Type == TokenType.EOF;
    Token Consume()
    {
        if (!IsAtEnd) Current++;
        return PreviousToken;
    }
    bool Check(TokenType type)
    {
        if (IsAtEnd) return false;
        return CurrentToken.Type == type;
    }

    bool MatchAny(params TokenType[] types)
    {
        foreach (var type in types)
        {
            if (Check(type))
            {
                Consume();
                return true;
            }
        }
        return false;
    }
}