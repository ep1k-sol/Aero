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

    private class ParseError : Exception { }

    // parse
    public List<Stmt> Parse()
    {
        var statements = new List<Stmt>();

        try
        {
            while (!IsAtEnd())
            {
                statements.Add(Statement());
            }
        }
        catch (ParseError)
        {
            Synchronize();
            Parse();
        }

        return statements;
    }

    // 코드블록
    List<Stmt> ParseBlock()
    {
        Consume(TokenType.LEFT_BRACE, Errors.MISSING_BRACE);
        var statements = new List<Stmt>();

        while (!CheckNext(TokenType.RIGHT_BRACE))
        {
            statements.Add(Statement());
        }

        Consume(TokenType.RIGHT_BRACE, Errors.UNTERMINATED_BRACE);
        return statements;
    }

    // returns list of nodes separated with ','
    List<T> ParseSeparatedValues<T>(TokenType end, Func<T> func, string error)
    {
        var result = new List<T>();

        while (!CheckNext(end))
        {
            var node = func();

            result.Add(node);

            if (!CheckNext(end)) Consume(TokenType.COMMA, error);
        }

        Consume(end, error);
        return result;
    }

    // parse arguments
    List<Expr> Argument()
    {
        Console.WriteLine(GetNext());
        Consume(TokenType.LEFT_PAREN, Errors.MISSING_PARENTHESIS);

        var arg = ParseSeparatedValues<Expr>(TokenType.RIGHT_PAREN, Expression, Errors.MISSING_PARENTHESIS);
        return arg;
    }

    // parse parameters
    List<object> Parameter()
    {
        Consume(TokenType.LEFT_PAREN, Errors.MISSING_PARENTHESIS);

        var param = ParseSeparatedValues<object>(TokenType.RIGHT_PAREN, Expression, Errors.MISSING_PARENTHESIS);
        return param;
    }


    // [ PARSING ] //
    Stmt Statement()
    {
        var token = GetNext();

        switch (token.type)
        {
            case TokenType.PRINT: return Print();
            case TokenType.LOCAL: return LocalDecl();
            case TokenType.GLOBAL: return GlobalDecl();

            default:
                return ExprStmt();
        }
    }

    // Exprstmt
    Stmt ExprStmt()
    {
        var expr = Expression();

        return new ExprStmt(expr);
    }

    // declaration
    // "local" "func" (Identifier) "(" (Identifier ",")* ")" "{" (Statement)* "}"
    Stmt LocalDecl()
    {
        var scope = Advance();

        if (CheckNext(TokenType.IDENTIFIER))
        {
            var name = Advance().literal;
            Expr? initializer = null;

            if (CheckNext(TokenType.EQUAL))
            {
                Consume(TokenType.EQUAL, Errors.MISSING_EQUAL);

                initializer = Expression();
            }

            return new Variable(name, initializer, scope);
        }
        else if (CheckNext(TokenType.FUNCTION))
        {
            Advance();

            var name = Advance().literal;
            var param = Parameter();
            var code = ParseBlock();

            return new Function(name, param, code, scope);
        }

        Error(Advance(), Errors.UNEXPECTED_DECL);
        return new Invalid();
    }

    // "global" "func" (Identifier) "(" (Identifier ",")* ")" "{" (Statement)* "}"
    Stmt GlobalDecl()
    {
        var scope = Advance();

        if (CheckNext(TokenType.IDENTIFIER))
        {
            var name = Advance().literal;
            Expr? initializer = null;

            if (CheckNext(TokenType.EQUAL))
            {
                Consume(TokenType.EQUAL, Errors.MISSING_EQUAL);

                initializer = Expression();
            }

            return new Variable(name, initializer, scope);
        }
        else if (CheckNext(TokenType.FUNCTION))
        {
            Advance();

            var name = Advance().literal;
            var param = Parameter();
            var code = ParseBlock();

            return new Function(name, param, code, scope);
        }

        Error(Advance(), Errors.UNEXPECTED_DECL);
        return new Invalid();
    }

    // "print" "(" Expression ")"
    Stmt Print()
    {
        Advance();

        var value = Argument();

        return new Print(value);
    }

    // expression go brrr
    Expr Expression()
    {
        return Equality();
    }


    // Comparison ( ( "!=" | "==" ) Comparison )* 
    Expr Equality()
    {
        var left = Comparison();

        while (CheckNext(TokenType.BANG_EQUAL) || CheckNext(TokenType.EQUAL_EQUAL))
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

        while (CheckNext(TokenType.GREATER_EQUAL) || CheckNext(TokenType.LESS_EQUAL) || CheckNext(TokenType.GREATER) || CheckNext(TokenType.LESS))
        {
            var op = Advance();
            var right = Term();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // Factor ( ( "+" | "-" | ".." ) Factor )*
    Expr Term()
    {
        var left = Factor();

        while (CheckNext(TokenType.PLUS) || CheckNext(TokenType.MINUS) || CheckNext(TokenType.DOTDOT))
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

        while (CheckNext(TokenType.STAR) || CheckNext(TokenType.SLASH) || CheckNext(TokenType.MODULO))
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

        while (CheckNext(TokenType.POWER))
        {
            var op = Advance();
            var right = Unary();

            left = new Binary(left, op, right);
        }

        return left;
    }

    // ( "!" | "+" | "-" ) Unary | Primary
    Expr Unary()
    {
        if (CheckNext(TokenType.BANG) || CheckNext(TokenType.PLUS) || CheckNext(TokenType.MINUS))
        {
            var op = Advance();
            var right = Unary();

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

            case TokenType.NUMBER or TokenType.STRING or TokenType.IDENTIFIER: return new Literal(token.literal);
            case TokenType.LEFT_PAREN:
                {
                    var expr = Term();
                    Consume(TokenType.RIGHT_PAREN, Errors.UNTERMINATED_PARENTHESIS);

                    return new Group(expr);
                }
            default:
                if (CheckNextNext(TokenType.LEFT_PAREN))
                {
                    Console.WriteLine("여길 오나?");
                    var args = Argument();

                    return new Call(token.lexeme, args);
                }
                else
                {
                    Console.WriteLine($"from parser: {token}");
                    throw Error(token, Errors.UNEXPECTED_LITERAL);
                }
        }
    }

    void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            switch (GetNext().type)
            {
                case TokenType.PRINT:
                case TokenType.LOCAL:
                case TokenType.GLOBAL:
                case TokenType.FUNCTION:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    // error
    ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    // consumes current token if match, or error
    void Consume(TokenType expected, string error)
    {
        var token = GetNext();

        if (token.type == expected)
        {
            current++;
        }
        else
        {
            Error(token, error);
        }
    }

    // yeah.
    Token GetNext()
    {
        return _tokens[current];
    }

    Token GetNextNext()
    {
        return _tokens[current + 1];
    }

    // returns true if CURRENT token matchs
    bool CheckNext(TokenType expected)
    {
        if (IsAtEnd()) return false;
        if (GetNext().type != expected) return false;

        return true;
    }

    // returns true if NEXT token matchs
    bool CheckNextNext(TokenType expected)
    {
        if (IsAtEnd()) return false;
        if (GetNextNext().type != expected) return false;

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