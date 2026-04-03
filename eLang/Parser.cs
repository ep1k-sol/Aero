namespace eLang;

class Parser
{
    readonly List<Token> _tokens;
    int current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    public Expr Parse()
    {
        return Expression();
    }

    Expr Expression()
    {
        var left = Term();
        var token = _tokens[current];

        if (token.Is(TokenType.PLUS) || token.Is(TokenType.MINUS))
        {
            var op = Advance();
            var right = Term();

            return new Binary(left, op, right);
        }

        return left;
    }

    Expr Term()
    {
        var left = Primary();
        var token = _tokens[current];

        if (token.Is(TokenType.STAR) || token.Is(TokenType.SLASH))
        {
            var op = Advance();
            var right = Primary();

            return new Binary(left, op, right);
        }

        return left;
    }

    Expr Primary()
    {
        var node = Advance();

        if (node.Is(TokenType.NUMBER))
        {
            return new Literal(node);
        }
        else if (node.Is(TokenType.LEFT_PAREN))
        {
            return 
        }
    }

    TokenType CheckNext()
    {
        if (IsAtEnd()) return TokenType.EOF;

        return _tokens[current].type;
    }

    Token Advance()
    {
        return _tokens[current++];
    }

    bool IsAtEnd()
    {
        return (_tokens[current].type == TokenType.EOF);
    }
}