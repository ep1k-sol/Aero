namespace Aero.AST;

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
    public Token name { get; init; }
    public Expr? value { get; init; }
    public Token scope { get; init; }

    public Variable(Token name, Expr? value, Token scope)
    {
        this.name = name;
        this.value = value;
        this.scope = scope;
    }
}

class Block : Stmt
{
    public List<Stmt> code { get; init; }

    public Block(List<Stmt> code)
    {
        this.code = code;
    }
}
class Function : Stmt
{
    public Token name { get; init; }
    public List<Token> param { get; init; }
    public Block block { get; init; }
    public Token scope { get; init; }

    public Function(Token name, List<Token> param, Block block, Token scope)
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
    public List<Expr> value { get; init; }

    public Print(List<Expr> value)
    {
        this.value = value;
    }
}

// if
class If : Stmt
{
    public Expr condition { get; init; }
    public Block block { get; init; }
    public Stmt? elseBranch { get; init; }

    public If(Expr condition, Block block, Stmt? elses)
    {
        this.condition = condition;
        this.block = block;
        this.elseBranch = elses;
    }
}

// loop
class While : Stmt
{
    public Expr condition { get; init; }
    public Block block { get; init; }

    public While(Expr condition, Block block)
    {
        this.condition = condition;
        this.block = block;
    }
}

class Break : Stmt
{
    public Token keyword { get; init; }

    public Break(Token keyword)
    {
        this.keyword = keyword;
    }
}
class For : Stmt
{
    public Stmt init { get; init; }
    public Expr condition { get; init; }
    public Expr step { get; init; }
    public Block block { get; init; }

    public For(Stmt init, Expr condition, Expr step, Block block)
    {
        this.init = init;
        this.condition = condition;
        this.step = step;
        this.block = block;
    }
}

// return
class Return : Stmt
{
    public Token keyword { get; init; }
    public Expr? value { get; init; }

    public Return(Token keyword, Expr? value)
    {
        this.keyword = keyword;
        this.value = value;
    }
}
