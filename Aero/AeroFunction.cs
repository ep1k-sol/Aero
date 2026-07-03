using Aero.AST;

namespace Aero;

class StdFunction
{
    public readonly string name;
    readonly Func<Token, List<AeroValue>, AeroValue> func;

    public StdFunction(string name, Func<Token, List<AeroValue>, AeroValue> func)
    {
        this.name = name;
        this.func = func;
    }

    public AeroValue Call(Token token, List<AeroValue> args) => func(token, args);
}

class AeroFunction
{
    public readonly List<Token> param;
    public readonly Block body;
    public readonly EnvironmentTable closure;

    public AeroFunction(List<Token> param, Block body, EnvironmentTable closure)
    {
        this.param = param;
        this.body = body;
        this.closure = closure;
    }

    public int Arity => param.Count;
}
