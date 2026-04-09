namespace eLang;

abstract class Expr {}

// 문자 or 숫자
class Literal : Expr
{
    public object? value;
    public Literal(object? value) { this.value = value; }

    public override string ToString()
    {
        return $"\nVALUE [ {value} ]";
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

    public override string ToString()
    {
        return $"\nBinary [ \nleft: {left}  |  \nop: {op}  |  \nright:{right} ]";
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

    public override string ToString()
    {
        return $"\nUNARY [ \nop: {op}  |  \nright: {right} ]";
    }
}

// 숫자, 괄호
class Group : Expr
{
    public Expr paren;
    public Group(Expr paren) { this.paren = paren; }

    public override string ToString()
    {
        return $"\nGROUP [ \n({paren}) ]";
    }
}