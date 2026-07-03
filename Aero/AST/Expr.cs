namespace Aero.AST;

abstract class Expr { }

// 문자 or 숫자
class Literal : Expr
{
    public object? value { get; init; }
    public Literal(object? value) { this.value = value; }
}

class Assign : Expr
{
    public Expr target { get; init; }
    public Token token { get; init; }
    public Expr value { get; init; }

    public Assign(Expr target, Token token, Expr value)
    {
        this.target = target;
        this.token = token;
        this.value = value;
    }
}

class Lambda : Expr
{
    public List<Token> param { get; init; }
    public Block block { get; init; }

    public Lambda(List<Token> param, Block block)
    {
        this.param = param;
        this.block = block;
    }
}

class Call : Expr
{
    public Expr callee { get; init; }
    public Token token { get; init; }
    public List<Expr> args { get; init; }

    public Call(Expr callee, Token token, List<Expr> args)
    {
        this.callee = callee;
        this.token = token;
        this.args = args;
    }
}

// 항
class Binary : Expr
{
    public Expr left { get; init; }
    public Token op { get; init; }
    public Expr right { get; init; }
    public Binary(Expr left, Token op, Expr right)
    {
        this.left = left;
        this.op = op;
        this.right = right;
    }
}

// 부호
class Unary : Expr
{
    public Token op { get; init; }
    public Expr right { get; init; }
    public Unary(Token op, Expr right)
    {
        this.op = op;
        this.right = right;
    }
}

class Postfix : Expr
{
    public Expr left { get; init; }
    public Token op { get; init; }

    public Postfix(Expr left, Token op)
    {
        this.left = left;
        this.op = op;
    }
}

// variable ITSELF
class VariableExpr : Expr
{
    public Token value { get; init; }
    public VariableExpr(Token value) { this.value = value; }
}

// 숫자, 괄호
class Group : Expr
{
    public Expr paren { get; init; }
    public Group(Expr paren) { this.paren = paren; }
}

class ArrayLiteral : Expr
{
    public List<Expr> elements { get; init; }

    public ArrayLiteral(List<Expr> body) { this.elements = body; }
}

class IndexExpr : Expr
{
    public Expr target { get; init; }
    public Token bracket { get; init; }
    public Expr index { get; init; }

    public IndexExpr(Expr target, Token bracket, Expr index)
    {
        this.target = target;
        this.bracket = bracket;
        this.index = index;
    }
}

class IndexAssign : Expr
{
    public Expr target { get; init; }
    public Expr index { get; init; }
    public Expr value { get; init; }
    public Token bracket { get; init; }

    public IndexAssign(Expr target, Token bracket, Expr index, Expr value)
    {
        this.target = target;
        this.bracket = bracket;
        this.index = index;
        this.value = value;
    }
}

class DictLiteral : Expr
{
    public List<(Token key, Expr value)> pairs { get; init; }
    public DictLiteral? alternative { get; set; }

    public DictLiteral(List<(Token key, Expr value)> pairs, DictLiteral? alternative)
    {
        this.pairs = pairs;
        this.alternative = alternative;
    }
}

class FieldExpr : Expr
{
    public Expr target { get; init; }
    public Token dot { get; init; }
    public Token name { get; init; }

    public FieldExpr(Expr target, Token dot, Token name)
    {
        this.target = target;
        this.dot = dot;
        this.name = name;
    }
}

class FieldAssign : Expr
{
    public Expr target { get; init; }
    public Token dot { get; init; }
    public Token name { get; init; }
    public Expr value { get; init; }

    public FieldAssign(Expr target, Token dot, Token name, Expr value)
    {
        this.target = target;
        this.dot = dot;
        this.name = name;
        this.value = value;
    }
}