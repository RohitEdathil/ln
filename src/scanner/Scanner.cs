
class Scanner
{
    ErrorHandler errorHandler;
    string code;
    int currentPos = 0;
    int line = 1;

    char CurrentChar => code[currentPos];
    char NextChar => IsAtEnd() ? '\0' : code[currentPos + 1];

    readonly List<Token> tokens = new();

    public Scanner(string code, ErrorHandler errorHandler)
    {
        this.code = code + '\0';
        this.errorHandler = errorHandler;
    }

    void AddToken(TokenType type, object? literal = null)
    {
        tokens.Add(new Token(type, literal, line));
    }

    bool IsAtEnd()
    {
        return currentPos >= code.Length - 1;
    }

    void Advance(int steps = 1)
    {
        for (int step = 0; step < steps; step++)
        {
            if (CurrentChar == '\n')
            {
                line++;
            }
            currentPos++;
        }
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            ScanToken();
        }
        AddToken(TokenType.EOF);
        return tokens;
    }

    int SkipUntil(Func<char, bool> condition)
    {
        int start = currentPos;
        while (!IsAtEnd() && !condition(CurrentChar))
        {
            Advance();
        }

        return currentPos - start;
    }

    void ScanToken()
    {
        // Single-character tokens
        Dictionary<char, TokenType> singleCharTokenMappings =
            new()
            {
                {'(', TokenType.LEFT_PAREN},
                {')', TokenType.RIGHT_PAREN},
                {'{', TokenType.LEFT_BRACE},
                {'}', TokenType.RIGHT_BRACE},
                {',', TokenType.COMMA},
                {'.', TokenType.DOT},
                {'-', TokenType.MINUS},
                {'+', TokenType.PLUS},
                {';', TokenType.SEMICOLON},
                {'*', TokenType.STAR},
            };

        if (singleCharTokenMappings.ContainsKey(CurrentChar))
        {
            AddToken(singleCharTokenMappings[CurrentChar]);
            Advance();
            return;
        }

        // Maybe equal suffixed tokens
        Dictionary<char, (TokenType, TokenType)> maybeEqualSuffixedTokenMappings =
            new()
            {
                {'!', (TokenType.BANG,TokenType.BANG_EQUAL)},
                {'=', (TokenType.EQUAL,TokenType.EQUAL_EQUAL)},
                {'<', (TokenType.LESS,TokenType.LESS_EQUAL)},
                {'>', (TokenType.GREATER,TokenType.GREATER_EQUAL)},
            };

        if (maybeEqualSuffixedTokenMappings.ContainsKey(CurrentChar))
        {
            if (NextChar == '=')
            {
                AddToken(maybeEqualSuffixedTokenMappings[CurrentChar].Item2);
                Advance(2);
                return;
            }
            AddToken(maybeEqualSuffixedTokenMappings[CurrentChar].Item1);
            Advance();
            return;
        }

        // Slash or comment
        if (CurrentChar == '/')
        {
            if (NextChar == '/')
            {
                // Comment
                Advance(2);
                SkipUntil(c => c == '\n');
                return;
            }
            AddToken(TokenType.SLASH);
            Advance();
            return;
        }

        // Whitespaces
        if (CurrentChar == ' ' || CurrentChar == '\r' || CurrentChar == '\t' || CurrentChar == '\n')
        {
            Advance();
            return;
        }

        // String literal
        if (CurrentChar == '"')
        {
            Advance();
            int start = currentPos;
            int length = SkipUntil(c => c == '"');
            if (IsAtEnd())
            {
                errorHandler.Error(line, "Unterminated string");
                return;
            }
            string literal = code.Substring(start, length);
            AddToken(TokenType.STRING, literal: literal);
            Advance();
            return;
        }

        // Number literal
        if (char.IsDigit(CurrentChar))
        {
            int start = currentPos;
            int length = SkipUntil(c => !char.IsDigit(c));
            if (length == -1)
            {
                length = code.Length - start - 1;
            }
            if (CurrentChar == '.')
            {
                length += 1;
                Advance();
                length += SkipUntil(c => !char.IsDigit(c));
            }
            string literal = code.Substring(start, length);
            AddToken(TokenType.NUMBER, literal: double.Parse(literal));
            return;
        }

        // Identifier or keyword
        Dictionary<string, TokenType> keywordMappings =
            new()
            {
                {"and",TokenType.AND},
                {"class",TokenType.CLASS},
                {"else",TokenType.ELSE},
                {"false",TokenType.FALSE},
                {"for",TokenType.FOR},
                {"fun",TokenType.FUN},
                {"if",TokenType.IF},
                {"nil",TokenType.NIL},
                {"or",TokenType.OR},
                {"print",TokenType.PRINT},
                {"return",TokenType.RETURN},
                {"super",TokenType.SUPER},
                {"this",TokenType.THIS},
                {"true",TokenType.TRUE},
                {"var",TokenType.VAR},
                {"while",TokenType.WHILE},
            };

        if (CurrentChar == '_' || char.IsLetter(CurrentChar))
        {
            int start = currentPos;
            int length = SkipUntil(c => !char.IsLetterOrDigit(c) && c != '_');
            if (length == -1)
            {
                length = code.Length - start;
            }
            string literal = code.Substring(start, length);

            if (keywordMappings.ContainsKey(literal))
            {
                AddToken(keywordMappings[literal]);
            }
            else
            {
                AddToken(TokenType.IDENTIFIER, literal: literal);
            }
            return;

        }

        // Unkown character
        errorHandler.Error(line, "Unexpected character : " + CurrentChar);
        Advance();

    }

}