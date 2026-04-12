using eLang.AST;

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

    // Term ( ( "+" | "-" ) Term )*
    Expr Expression()
    {
        var left = Term();
        var token = CheckNext();

        while (token.Is(TokenType.PLUS) || token.Is(TokenType.MINUS))
        {
            var op = Advance();
            var right = Term();

            left = new Binary(left, op, right);
            token = CheckNext();
        }

        return left;
    }

    // Modulo ( ( "*" | "/" ) Modulo )*
    Expr Term()
    {
        var left = Power();
        var token = CheckNext();

        while (token.Is(TokenType.STAR) || token.Is(TokenType.SLASH) || token.Is(TokenType.MODULO))
        {
            var op = Advance();
            var right = Power();

            left = new Binary(left, op, right);
            token = CheckNext();
        }

        return left;
    }

    // Unary ( "^" Unary )*
    Expr Power()
    {
        var left = Unary();
        var token = CheckNext();

        while (token.Is(TokenType.POWER))
        {
            var op = Advance();
            var right = Unary();

            left = new Binary(left, op, right);
            token = CheckNext();
        }

        return left;
    }

    // ( "+" | "-" ) Unary | Primary
    Expr Unary()
    {
        if (CheckNext().Is(TokenType.MINUS) || CheckNext().Is(TokenType.PLUS))
        {
            var op = Advance();
            var right = Primary();
            return new Unary(op, right);
        }

        return Primary();
    }

    // NUMBER | "(" Expression ")"
    Expr Primary()
    {
        if (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();
            Consume(TokenType.RIGHT_PAREN, Errors.UNTERMINATED_PARENTHESIS);
            return new Group(expr);
        }

        var node = Advance();
        return new Literal(node.literal);
    }


    void Consume(TokenType type, string error)
    {
        if (_tokens[current].type == type)
        {
            current++;
        }
        else
        {
            Program.Error(_tokens[current].line, error);
        }
    }

    Token CheckNext()
    {
        return _tokens[current];
    }

    bool Match(TokenType expected)
    {
        if (IsAtEnd()) return false;
        if (_tokens[current].type != expected) return false;

        current++;
        return true;
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