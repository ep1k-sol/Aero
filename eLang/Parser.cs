using System.Runtime.InteropServices;

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
            Console.WriteLine("at expression");
            var right = Term();

            left = new Binary(left, op, right);
            token = CheckNext();
        }

        return left;
    }

    // Factor ( ( "*" | "/" ) Factor )*
    Expr Term()
    {
        var left = Factor();
        var token = CheckNext();

        while (token.Is(TokenType.STAR) || token.Is(TokenType.SLASH))
        {
            var op = Advance();
            Console.WriteLine("at term");
            var right = Factor();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // ( "+" | "-" ) Factor | Primary
    Expr Factor()
    {
        while (CheckNext().Is(TokenType.MINUS) || CheckNext().Is(TokenType.PLUS))
        {
            var op = Advance();
            Console.WriteLine("at factor");

            var right = Primary();

            return new Unary(op, right);
        }

        return Primary();
    }

    // NUMBER | "(" Expression ")"
    Expr Primary()
    {
        while (Match(TokenType.LEFT_PAREN))
        {
            var expr = Expression();

            Consume(TokenType.RIGHT_PAREN, Errors.UNTERMINATED_PARENTHESIS);

            return new Group(expr);
        }

        var node = Advance();
        Console.WriteLine("at primary");

        return new Literal(node);

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