using eLang.AST;

namespace eLang;

static class Integrated
{
    public static readonly Dictionary<string, Func<List<Expr>, object?>> keywords = new()
    {
        {"input", (args) => Console.ReadLine()}
    };
}
