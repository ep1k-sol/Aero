using eLang.AST;

namespace eLang;

static class Debug
{
    public static void PrintTokens(List<Token> tokens)
    {
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }

    public static void PrintAST(List<Stmt> ast)
    {
        Console.WriteLine("\nAST result\n");
        PrintStmts(ast);
    }
    static void PrintStmts(List<Stmt> ast, int depth = 0)
    {
        foreach (Stmt stmt in ast)
        {
            string indent = new string(' ', depth * 2);

            if (stmt is ExprStmt e)
            {
                PrintExprAST(e.expr);
            }

            if (stmt is Print p)
            {
                Console.WriteLine($"{indent}PRINT (");
                foreach (Expr expr in p.value)
                {
                    PrintExprAST(expr, depth + 2);
                }
                Console.WriteLine($"{indent})");
            }

            if (stmt is Variable v)
            {
                Console.WriteLine($"{indent} VARIABLE: {v.scope.lexeme}, {v.name}, VALUE: [");
                PrintExprAST(v.value, depth + 2);
                Console.WriteLine($"{indent}]");
            }

            if (stmt is Function f)
            {
                Console.WriteLine($"{indent} FUNCTION: [");
                Console.WriteLine($"{indent}    SCOPE: [ {f.scope.lexeme} ]");
                Console.WriteLine($"{indent}    NAME: [ {f.name} ]");
                Console.WriteLine($"{indent}    PARAMETERS: [");
                foreach (var id in f.param)
                {
                    Console.WriteLine($"{id}", depth + 2);
                }
                Console.WriteLine($"{indent}    ]");
                Console.WriteLine($"{indent}    CODE: [");
                PrintStmts(f.code, depth + 2);
                Console.WriteLine($"{indent}    ]");
            }
        }
    }
    static void PrintExprAST(Expr? expr, int depth = 0)
    {
        string indent = new string(' ', depth * 2);

        if (expr is Literal l)
        {
            Console.WriteLine($"{indent}LITERAL: {l.value}");
            return;
        }

        if (expr is Call c)
        {
            Console.WriteLine($"{indent}CALL: {c.name}()");
        }

        if (expr is Binary b)
        {
            Console.WriteLine($"{indent}BINARY [");
            Console.WriteLine($"{indent}    left [");
            PrintExprAST(b.left, depth + 2);
            Console.WriteLine($"{indent}    ]");
            Console.WriteLine($"{indent}    op [ {b.op.lexeme} ]");
            Console.WriteLine($"{indent}    right [");
            PrintExprAST(b.right, depth + 2);
            Console.WriteLine($"{indent}    ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is Unary u)
        {
            Console.WriteLine($"{indent}UNARY [");
            Console.WriteLine($"{indent}    op [ {u.op.lexeme} ]");
            Console.WriteLine($"{indent}    right [");
            PrintExprAST(u.right, depth + 2);
            Console.WriteLine($"{indent}    ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is Group g)
        {
            Console.WriteLine($"{indent}GROUP (");
            PrintExprAST(g.paren, depth + 2);
            Console.WriteLine($"{indent})");
        }
    }

    public static void PrintEvaluated(object? result)
    {
        Console.WriteLine(result);
    }
}
