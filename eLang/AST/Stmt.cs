namespace eLang.AST;

abstract class Stmt { }


// declaration
class Local : Stmt
{
    public object? name;
    public Expr? value;

    public Local(object? name, Expr? value)
    {
        this.name = name;
        this.value = value;
    }
}

class Global : Stmt
{
    public object? name;
    public Expr? value;

    public Global(object? name, Expr? value)
    {
        this.name = name;
        this.value = value;
    }
}

// print
class Print : Stmt
{
    public Expr? value;

    public Print(Expr? value)
    {
        this.value = value;
    }
}

// if
class If : Stmt
{
    public Stmt? condition;
    public Stmt? block;

    public If(Stmt? condition, Stmt? block)
    {
        this.condition = condition;
        this.block = block;
    }
}

// loop
class While : Stmt
{
    public Stmt? condition;
    public Stmt? block;

    public While(Stmt? condition, Stmt? block)
    {
        this.condition = condition;
        this .block = block;
    }
}

class For : Stmt
{
    public Stmt? condition;
    public Stmt? block;

    public For(Stmt? condition, Stmt? block)
    {
        this.condition = condition;
        this.block = block;
    }
}