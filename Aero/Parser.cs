using Aero.AST;

namespace Aero;

class Parser
{
    readonly List<Token> _tokens;
    int _current = 0;

    public Parser(List<Token> tokens)
    {
        _tokens = tokens;
    }

    private class ParseError : Exception { }


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

    Stmt Statement()
    {
        var token = GetNext();

        switch (token.type)
        {
            case TokenType.LOCAL: return LocalDecl();
            case TokenType.GLOBAL: return GlobalDecl();
            case TokenType.IF: return If();
            case TokenType.WHILE: return While();
            case TokenType.BREAK: return Break();
            case TokenType.FOR: return For();
            case TokenType.RETURN: return Return();

            default:
                return ExprStmt();
        }
    }


    const int PREFIX_BP = 80;
    int GetLbp(TokenType type) => type switch
    {
        TokenType.EQUAL => 10,
        TokenType.OR => 15,
        TokenType.AND => 20,
        TokenType.EQUAL_EQUAL or TokenType.BANG_EQUAL or
        TokenType.LESS or TokenType.LESS_EQUAL or
        TokenType.GREATER or TokenType.GREATER_EQUAL => 30,
        TokenType.DOTDOT => 40,

        TokenType.PLUS or TokenType.MINUS => 50,
        TokenType.STAR or TokenType.SLASH or TokenType.MODULO => 60,
        TokenType.POWER => 70,
        TokenType.DOT => 95,
        TokenType.PLUS_PLUS or TokenType.MINUS_MINUS => 95,
        TokenType.LEFT_PAREN or TokenType.LEFT_BRACKET or TokenType.LEFT_BRACE => 90,
        _ => 0
    };

    int GetRbp(TokenType type) => type switch
    {
        // right-associative
        TokenType.EQUAL => 9,
        TokenType.DOTDOT => 39,
        TokenType.POWER => 69,

        // left-associative
        TokenType.OR => 15,
        TokenType.AND => 20,
        TokenType.EQUAL_EQUAL or TokenType.BANG_EQUAL or
        TokenType.LESS or TokenType.LESS_EQUAL or
        TokenType.GREATER or TokenType.GREATER_EQUAL => 30,
        TokenType.PLUS or TokenType.MINUS => 50,
        TokenType.STAR or TokenType.SLASH or TokenType.MODULO => 60,
        _ => 0
    };

    // Expression
    Expr Expression()
    {
        return ParseExpression(0);
    }

    Expr ParseExpression(int minBp)
    {
        var token = Advance();
        var left = Nud(token);

        while (GetLbp(GetNext().type) > minBp)
        {
            var op = Advance();
            left = Led(left, op);
        }

        return left;
    }

    // nud (null denotation)
    Expr Nud(Token token)
    {
        switch (token.type)
        {
            // Literal
            case TokenType.TRUE: return new Literal(true);
            case TokenType.FALSE: return new Literal(false);
            case TokenType.NIL: return new Literal(null);
            case TokenType.NUMBER:
            case TokenType.STRING: return new Literal(token.literal);

            // Variable
            case TokenType.IDENTIFIER:
                return new VariableExpr(token);

            // Group
            case TokenType.LEFT_PAREN:
                {
                    var expr = ParseExpression(0);
                    Consume(TokenType.RIGHT_PAREN, "Expect ')' after expression.");

                    return new Group(expr);
                }

            // ArrayLiteral
            case TokenType.LEFT_BRACKET:
                {
                    var body = Array();
                    return new ArrayLiteral(body);
                }

            // DictLiteral
            case TokenType.LEFT_BRACE:
                {
                    var pairs = new List<(Token key, Expr value)>();

                    while (!CheckNext(TokenType.RIGHT_BRACE))
                    {
                        var key = ConsumeKey();

                        Consume(TokenType.COLON, "Expect ':' after dictionary key.");

                        var value = Expression();

                        pairs.Add((key, value));

                        if (!CheckNext(TokenType.RIGHT_BRACE))
                            Consume(TokenType.COMMA, "Expect ',' between dictionary pairs.");
                    }

                    Consume(TokenType.RIGHT_BRACE, "Expect '}' after dictionary.");
                    return new DictLiteral(pairs, null);
                }

            // Prefix: !, +, -
            case TokenType.BANG:
            case TokenType.PLUS:
            case TokenType.MINUS:
                {
                    var right = ParseExpression(PREFIX_BP);
                    return new Unary(token, right);
                }

            // Prefix increment/decrement: ++x, --x
            case TokenType.PLUS_PLUS:
            case TokenType.MINUS_MINUS:
                {
                    var right = ParseExpression(PREFIX_BP);

                    if (right is not VariableExpr)
                        throw Error(token, "Prefix '++' / '--' target must be a variable.");

                    return new Unary(token, right);
                }

            case TokenType.FUNCTION:
                {
                    var param = Parameter();
                    var block = ParseBlock();
                    return new Lambda(param, block);
                }

            default:
                throw Error(token, $"Unexpected token '{token.lexeme}'.");
        }
    }

    // led (left denotation)
    Expr Led(Expr left, Token op)
    {
        switch (op.type)
        {
            // Assignment
            case TokenType.EQUAL:
                {
                    var value = ParseExpression(GetRbp(op.type));

                    if (left is VariableExpr v)
                        return new Assign(v, op, value);
                    if (left is IndexExpr idx)
                        return new IndexAssign(idx.target, idx.bracket, idx.index, value);
                    if (left is FieldExpr f)
                        return new FieldAssign(f.target, f.dot, f.name, value);

                    throw Error(op, "Invalid assignment target.");
                }

            // Postfix increment/decrement: x++, x--
            case TokenType.PLUS_PLUS:
            case TokenType.MINUS_MINUS:
                {
                    if (left is VariableExpr v) return new Postfix(v, op);

                    throw Error(op, "Postfix '++' / '--' target must be a variable.");
                }

            // and or
            case TokenType.AND:
            case TokenType.OR:
                {
                    var right = ParseExpression(GetRbp(op.type));
                    return new Binary(left, op, right);
                }

            // Binary Operator
            case TokenType.PLUS:
            case TokenType.MINUS:
            case TokenType.STAR:
            case TokenType.SLASH:
            case TokenType.MODULO:
            case TokenType.POWER:
            case TokenType.DOTDOT:
            case TokenType.EQUAL_EQUAL:
            case TokenType.BANG_EQUAL:
            case TokenType.GREATER:
            case TokenType.GREATER_EQUAL:
            case TokenType.LESS:
            case TokenType.LESS_EQUAL:
                {
                    var right = ParseExpression(GetRbp(op.type));
                    return new Binary(left, op, right);
                }

            // dictionary lookup
            case TokenType.DOT:
                {
                    var name = ConsumeIdentifier();
                    return new FieldExpr(left, op, name);
                }

            // Function Call
            case TokenType.LEFT_PAREN:
                {
                    var args = ParseSeparatedValues<Expr>(
                        TokenType.RIGHT_PAREN, Expression, "Expect ')' after arguments."
                    );
                    return new Call(left, op, args);
                }

            // Array indexing | Dictionary lookup
            case TokenType.LEFT_BRACKET:
                {
                    var idx = Expression();
                    Consume(TokenType.RIGHT_BRACKET, "Expect ']' after index.");
                    return new IndexExpr(left, op, idx);
                }

            default:
                throw Error(op, $"Unexpected token '{op.lexeme}'.");
        }
    }

    // Statements

    Stmt ExprStmt()
    {
        var expr = Expression();
        return new ExprStmt(expr);
    }

    Stmt LocalDecl()
    {
        var scope = Advance();

        if (CheckNext(TokenType.IDENTIFIER))
        {
            var name = Advance();
            Expr? initializer = null;

            if (CheckNext(TokenType.EQUAL))
            {
                Consume(TokenType.EQUAL, "Expect '=' after identifier.");
                initializer = Expression();
            }

            return new Variable(name, initializer, scope);
        }
        else if (CheckNext(TokenType.FUNCTION))
        {
            Advance();

            var name = Advance();
            var param = Parameter();
            var code = ParseBlock();

            return new Function(name, param, code, scope);
        }

        Error(Advance(), "Expect identifier or 'func' after scope keyword.");
        return new Invalid();
    }

    Stmt GlobalDecl()
    {
        var scope = Advance();

        if (CheckNext(TokenType.IDENTIFIER))
        {
            var name = Advance();
            Expr? initializer = null;

            if (CheckNext(TokenType.EQUAL))
            {
                Consume(TokenType.EQUAL, "Expect '=' after identifier.");
                initializer = Expression();
            }

            return new Variable(name, initializer, scope);
        }
        else if (CheckNext(TokenType.FUNCTION))
        {
            Console.WriteLine("whatthefuck");
            Advance();

            var name = Advance();
            var param = Parameter();
            var code = ParseBlock();

            return new Function(name, param, code, scope);
        }

        Error(Advance(), "Expect identifier or 'func' after scope keyword.");
        return new Invalid();
    }

    Stmt If()
    {
        Advance();
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'if'.");

        var condition = Expression();

        Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");

        var block = ParseBlock();

        Stmt? branch = null;

        if (CheckNext(TokenType.ELSE))
        {
            Advance();

            if (CheckNext(TokenType.IF))
                branch = Statement();
            else
                branch = ParseBlock();
        }

        return new If(condition, block, branch);
    }

    Stmt While()
    {
        Advance();
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'while'.");

        var condition = Expression();

        Consume(TokenType.RIGHT_PAREN, "Expect ')' after condition.");

        var block = ParseBlock();

        return new While(condition, block);
    }

    Stmt For()
    {
        Advance();
        Consume(TokenType.LEFT_PAREN, "Expect '(' after 'for'.");

        Stmt init;
        if (CheckNext(TokenType.LOCAL))
        {
            init = LocalDecl();
        }
        else if (CheckNext(TokenType.GLOBAL))
        {
            throw Error(Advance(), "Cannot declare global variable in for initializer.");
        }
        else
        {
            init = ExprStmt();
        }

        Consume(TokenType.SEMICOLON, "Expect ';' after for initializer.");

        var condition = Expression();
        Consume(TokenType.SEMICOLON, "Expect ';' after for condition.");
        var step = Expression();

        Consume(TokenType.RIGHT_PAREN, "Expect ')' after for clauses.");

        var block = ParseBlock();

        return new For(init, condition, step, block);
    }

    Stmt Break()
    {
        var keyword = Advance();

        return new Break(keyword);
    }

    Stmt Return()
    {
        var keyword = Advance();

        Expr? value = null;

        if (CanStartExpr(GetNext().type))
        {
            value = Expression();
        }

        return new Return(keyword, value);
    }

    // helpers

    bool CanStartExpr(TokenType type) => type switch
    {
        TokenType.TRUE or TokenType.FALSE or TokenType.NIL or
        TokenType.NUMBER or TokenType.STRING or TokenType.IDENTIFIER or
        TokenType.LEFT_PAREN or TokenType.BANG or
        TokenType.PLUS or TokenType.MINUS or
        TokenType.PLUS_PLUS or TokenType.MINUS_MINUS => true,
        _ => false
    };

    Block ParseBlock()
    {
        Consume(TokenType.LEFT_BRACE, "Expect '{' before block.");
        var statements = new List<Stmt>();

        while (!CheckNext(TokenType.RIGHT_BRACE))
        {
            statements.Add(Statement());
        }

        Consume(TokenType.RIGHT_BRACE, "Expect '}' after block.");
        return new Block(statements);
    }

    List<T> ParseSeparatedValues<T>(TokenType end, Func<T> func, string error)
    {
        var result = new List<T>();

        while (!CheckNext(end))
        {
            var node = func();
            result.Add(node);

            if (!CheckNext(end)) Consume(TokenType.COMMA, "Expect ',' between values.");
        }

        Consume(end, error);
        return result;
    }

    List<Expr> Argument()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' before arguments.");
        return ParseSeparatedValues<Expr>(TokenType.RIGHT_PAREN, Expression, "Expect ')' after arguments.");
    }

    List<Token> Parameter()
    {
        Consume(TokenType.LEFT_PAREN, "Expect '(' before parameters.");
        return ParseSeparatedValues<Token>(TokenType.RIGHT_PAREN, ConsumeIdentifier, "Expect ')' after parameters.");
    }

    List<Expr> Array()
    {
        return ParseSeparatedValues<Expr>(TokenType.RIGHT_BRACKET, Expression, "Expect ']' after array");
    }

    void Synchronize()
    {
        Advance();

        while (!IsAtEnd())
        {
            switch (GetNext().type)
            {
                case TokenType.LOCAL:
                case TokenType.GLOBAL:
                case TokenType.FUNCTION:
                case TokenType.IF:
                case TokenType.WHILE:
                case TokenType.BREAK:
                case TokenType.FOR:
                case TokenType.RETURN:
                    return;
            }

            Advance();
        }
    }

    ParseError Error(Token token, string message)
    {
        Program.Error(token, message);
        return new ParseError();
    }

    void Consume(TokenType expected, string error)
    {
        var token = GetNext();

        if (token.type == expected)
        {
            Advance();
        }
        else
        {
            Error(token, error);
        }
    }

    Token ConsumeIdentifier()
    {
        var token = GetNext();
        if (Keywords.keywords.TryGetValue(token.lexeme, out var keywords))
            throw Error(token, "Cannot use a reserved keyword as a name.");
        if (token.type != TokenType.IDENTIFIER)
            throw Error(token, "Parameter must be an identifier.");
        return Advance();
    }

    Token ConsumeKey()
    {
        if (CheckNext(TokenType.IDENTIFIER) || CheckNext(TokenType.STRING))
            return Advance();

        throw Error(GetNext(), "Dictionary key must be an identifier or a string.");
    }

    Token GetNext()
    {
        return _tokens[_current];
    }

    bool CheckNext(TokenType expected)
    {
        if (IsAtEnd()) return false;
        return GetNext().type == expected;
    }

    Token Advance()
    {
        return _tokens[_current++];
    }

    bool IsAtEnd()
    {
        return _tokens[_current].type == TokenType.EOF;
    }
}