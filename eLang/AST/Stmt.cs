namespace eLang.AST;

abstract class Stmt { }

class Invalid : Stmt
{

}

// idk
class ExprStmt : Stmt
{
    public Expr expr;

    public ExprStmt(Expr expr)
    {
        this.expr = expr;
    }
}

// declaration
class Variable : Stmt
{
    public object? name;
    public Expr? value;
    public Token scope;

    public Variable(object? name, Expr? value, Token scope)
    {
        this.name = name;
        this.value = value;
        this.scope = scope;
    }
}

class Block : Stmt
{
    public List<Stmt> code;

    public Block(List<Stmt> code)
    {
        this.code = code;
    }
}
class Function : Stmt
{
    public object? name;
    public List<object> param;
    public Block block;
    public Token scope;
    
    public Function(object? name, List<object> param, Block block, Token scope)
    {
        this.name = name;
        this.param = param;
        this.block = block;
        this.scope = scope;
    }
}

// print
class Print : Stmt
{
    public List<Expr> value;

    public Print(List<Expr> value)
    {
        this.value = value;
    }
}

// if
class If : Stmt
{
    public Expr condition;
    public Block block;

    public If(Expr condition, Block block)
    {
        this.condition = condition;
        this.block = block;
    }
}

// loop
class While : Stmt
{
    public Expr condition;
    public Block block;

    public While(Expr condition, Block block)
    {
        this.condition = condition;
        this.block = block;
    }
}

class For : Stmt
{
    public Expr condition;
    public Stmt block;

    public For(Expr condition, Block block)
    {
        this.condition = condition;
        this.block = block;
    }
}