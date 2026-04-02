namespace eLang;

abstract class Expr {}

class Literal : Expr
{
    public object? value;
    public Literal(object? value) { this.value = value; }
}

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

class Group : Expr
{
    public Expr paren;
    public Group(Expr paren) { this.paren = paren; }
}