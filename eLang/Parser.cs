using System.Runtime.CompilerServices;

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
        var token = _tokens[current];

        if (token.Is(TokenType.PLUS) || token.Is(TokenType.MINUS))
        {
            var op = Advance();
            var right = Term();

            return new Binary(left, op, right);
        }

        return left;
    }

    // Factor ( ( "*" | "/" ) Factor )*
    Expr Term()
    {
        var left = Factor();
        var token = _tokens[current];

        if (token.Is(TokenType.STAR) || token.Is(TokenType.SLASH))
        {
            var op = Advance();
            var right = Primary();

            return new Binary(left, op, right);
        }

        return left;
    }

    // ( "+" | "-" ) Factor | Primary
    Expr Factor()
    {
        if (_tokens[current].Is(TokenType.MINUS) || _tokens[current].Is(TokenType.PLUS))
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
        else
        {
            var node = Advance();

            return new Literal(node);
        }
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

    TokenType CheckNext()
    {
        if (IsAtEnd()) return TokenType.EOF;

        return _tokens[current].type;
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