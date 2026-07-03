namespace Aero;

class RuntimeError : Exception
{
    public readonly Token token;
    public RuntimeError(Token token, string message) : base(message)
    {
        this.token = token;
    }
}

class TypeError : RuntimeError
{
    public TypeError(Token token, string message) : base(token, message) { }
}

class NameError : RuntimeError
{
    public NameError(Token token, string message) : base(token, message) { }
}

class ZeroDivisionError : RuntimeError
{
    public ZeroDivisionError(Token token, string message) : base(token, message) { }
}
class IndexError : RuntimeError
{
    public IndexError(Token token, string message) : base(token, message) { }
}