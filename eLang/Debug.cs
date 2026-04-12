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

    public static void PrintAST(Expr ast, int depth = 0)
    {
        string indent = new string(' ', depth * 2);

        if (depth == 0) Console.WriteLine("\nAST result:\n");

        if (ast is Literal l)
        {
            Console.WriteLine($"{indent}LITERAL: {l.value}");
            return;
        }

        if (ast is Binary b)
        {
            Console.WriteLine($"{indent}BINARY [");
            Console.WriteLine($"{indent}    left [");
            PrintAST(b.left, depth + 2);
            Console.WriteLine($"{indent}    ]");
            Console.WriteLine($"{indent}    op [ {b.op.lexeme} ]");
            Console.WriteLine($"{indent}    right [");
            PrintAST(b.right, depth + 2);
            Console.WriteLine($"{indent}    ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (ast is Unary u)
        {
            Console.WriteLine($"{indent}UNARY [");
            Console.WriteLine($"{indent}    op [ {u.op.lexeme} ]");
            Console.WriteLine($"{indent}    right [");
            PrintAST(u.right, depth + 2);
            Console.WriteLine($"{indent}    ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (ast is Group g)
        {
            Console.WriteLine($"{indent}GROUP (");
            PrintAST(g.paren, depth + 2);
            Console.WriteLine($"{indent})");
        }
    }

    public static void PrintEvaluated(object? result)
    {
        Console.WriteLine(result);
    }
}
