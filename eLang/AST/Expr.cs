namespace eLang.AST;

abstract class Expr { }

// 문자 or 숫자
class Literal : Expr
{
    public object? value;
    public Literal(object? value) { this.value = value; }
}

class Assign : Expr
{
    public Token target;
    public Expr value;

    public Assign(Token target, Expr value)
    {
        this.target = target;
        this.value = value;
    }
}

class Call : Expr
{
    public string name;
    public List<Expr> args;
    public Call(string name, List<Expr> args)
    {
        this.name = name;
        this.args = args;
    }
}

// 항
class Binary : Expr
{
    public Expr left;
    public Token op;
    public Expr right;
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
    public Token op;
    public Expr right;
    public Unary(Token op, Expr right)
    {
        this.op = op;
        this.right = right;
    }
}

class VariableExpr : Expr
{
    public Token value;
    public VariableExpr(Token value) { this.value = value; }
}

// 숫자, 괄호
class Group : Expr
{
    public Expr paren;
    public Group(Expr paren) { this.paren = paren; }
}