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

    public static void PrintAST(Expr ast)
    {
        Console.WriteLine(ast.ToString());
    }

    public static void PrintEvaluated(object? result)
    {
        Console.WriteLine(result);
    }
}
