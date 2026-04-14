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

    /*
    Stmt Statement()
    {
        var token = Advance();

        switch (token.type)
        {
            case TokenType.LOCAL: Local(); break;

            case TokenType.GLOBAL: Global(); break;

            default:
                return;
        }
    }

    // local (identifier) = (expression)
    Stmt Local()
    {
        var name = 

        Consume(TokenType.EQUAL, Errors.MISSING_EQUAL);
        var value = Term();

        return new Local(name.literal, value);
    }

    // global (identifier) = (expression)
    Stmt Global()
    {
        var name = CheckNext().Is(TokenType.IDENTIFIER) ? Advance() : throw ;
        var value = Term();

        return new Local(name.literal, value);
    }
    */

    // expression go brrr
    Expr Expression()
    {
        return Equality();
    }


    // Comparison ( ( "!=" | "==" ) Comparison )* 
    Expr Equality()
    {
        var left = Comparison();

        while (MatchNext(TokenType.BANG_EQUAL) || MatchNext(TokenType.EQUAL_EQUAL))
        {
            var op = Advance();
            var right = Comparison();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // Term ( ( ">=" | "<=" | "<" | ">" ) Term )*
    Expr Comparison()
    {
        var left = Term();

        while (MatchNext(TokenType.GREATER_EQUAL) || MatchNext(TokenType.LESS_EQUAL) || MatchNext(TokenType.GREATER) || MatchNext(TokenType.LESS))
        {
            var op = Advance();
            var right = Term();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // Factor ( ( "+" | "-" ) Factor )*
    Expr Term()
    {
        var left = Factor();

        while (MatchNext(TokenType.PLUS) || MatchNext(TokenType.MINUS))
        {
            var op = Advance();
            var right = Factor();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // Power ( ( "*" | "/" ) Power )*
    Expr Factor()
    {
        var left = Power();

        while (MatchNext(TokenType.STAR) || MatchNext(TokenType.SLASH) || MatchNext(TokenType.MODULO))
        {
            var op = Advance();
            var right = Power();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // Unary ( "^" Unary )*
    Expr Power()
    {
        var left = Unary();

        while (MatchNext(TokenType.POWER))
        {
            var op = Advance();
            var right = Unary();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // ( "+" | "-" ) Unary | Primary
    Expr Unary()
    {
        if (MatchNext(TokenType.PLUS) || MatchNext(TokenType.MINUS))
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
        var token = Advance();

        switch (token.type)
        {
            case TokenType.TRUE: return new Literal(true);
            case TokenType.FALSE: return new Literal(false);
            case TokenType.NIL: return new Literal(null);
            case TokenType.LEFT_PAREN:
                {
                    var expr = Term();
                    Consume(TokenType.RIGHT_PAREN, Errors.UNTERMINATED_PARENTHESIS);

                    return new Group(expr);
                }

            default:
                return new Literal(token.literal);
        }
    }

    // consumes current token if match, or error
    void Consume(TokenType expected, string error)
    {
        if (_tokens[current].type == expected)
        {
            current++;
        }
        else
        {
            Program.Error(_tokens[current].line, error);
        }
    }

    // yeah.
    Token CheckNext()
    {
        return _tokens[current];
    }

    // returns true if match
    bool MatchNext(TokenType expected)
    {
        if (IsAtEnd()) return false;
        if (CheckNext().type != expected) return false;

        return true;
    }

    Token Advance()
    {
        return _tokens[current++];
    }

    //Token AdvanceIf(TokenType expected)
    //{
    //    if (!MatchNext(expected)) return;

    //    return Advance();
    //}

    bool IsAtEnd()
    {
        return (_tokens[current].type == TokenType.EOF);
    }
}