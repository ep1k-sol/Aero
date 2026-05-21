using eLang.AST;

namespace eLang;

class Evaluator
{
    EnvironmentTable globalEnv = new EnvironmentTable();
    EnvironmentTable currentEnv;

    public Evaluator()
    {
        currentEnv = globalEnv;
    }

    public void Evaluate(List<Stmt> stmts)
    {
        foreach (Stmt stmt in stmts)
        {
            if (stmt is Variable v)
            {
                var name = v.name;
                var scope = v.scope;
                var value = EvaluateExpr(v.value);

                if (name is null || scope is null) return;

                switch (scope.type)
                {
                    case TokenType.LOCAL: new EnvironmentTable(globalEnv).body.Add(name.ToString()!, value); break;
                    case TokenType.GLOBAL: globalEnv.body.Add(name.ToString()!, value); break;
                }
            }

            if (stmt is Function f)
            {
                Console.WriteLine("work in progress");
            }

            if (stmt is ExprStmt e)
            {
                EvaluateExpr(e.expr);
            }

            if (stmt is Print p)
            {
                var texts = new List<string?>();

                foreach (Expr expr in p.value)
                {
                    var value = EvaluateExpr(expr);
                    texts.Add($"{value}");
                }

                Console.WriteLine(string.Join('\t', texts));
            }
        }
    }

    private object? EvaluateExpr(Expr? expr)
    {
        if (expr is Literal l)
        {
            if (l.value is null) return null;
            if (l.value is string) return l.value;

            if (currentEnv.TryGetValue(l.value.ToString()!, out var value))
            {
                return value;
            }

            return l.value;
        }

        if (expr is Call c)
        {
            if (Integrated.keywords.TryGetValue(c.name, out var func))
            {
                return func(c.args);
            }
            else
            {
                return c;
            }
        }

        if (expr is Binary b)
        {
            var left = EvaluateExpr(b.left);
            var right = EvaluateExpr(b.right);

            if (left is null || right is null) return null;

            if (left is double leftNum) left = leftNum;
            if (right is double rightNum) right = rightNum;

            switch (b.op.type)
            {
                case TokenType.BANG_EQUAL: return (left != right);
                case TokenType.EQUAL_EQUAL: return (left == right);
                case TokenType.GREATER_EQUAL: return ((double)left >= (double)right);
                case TokenType.GREATER: return ((double)left > (double)right);
                case TokenType.LESS_EQUAL: return ((double)left <= (double)right);
                case TokenType.LESS: return ((double)left < (double)right);

                case TokenType.DOTDOT: return $"{left.ToString()}{right.ToString()}";
                case TokenType.PLUS: return (double)left + (double)right;
                case TokenType.MINUS: return (double)left - (double)right;
                case TokenType.STAR: return (double)left * (double)right;
                case TokenType.SLASH: return (double)left / (double)right;
                case TokenType.POWER: return Math.Pow((double)left, (double)right);
                case TokenType.MODULO: return (double)left % (double)right;
            }
        }

        if (expr is Unary u)
        {
            var right = EvaluateExpr(u.right);

            switch (u.op.type)
            {
                case TokenType.PLUS: return (double?)right;
                case TokenType.MINUS: return -(double?)right;
                case TokenType.BANG: return !IsTruthy(right);
            }
        }

        if (expr is Group g)
        {
            return EvaluateExpr(g.paren);
        }


        return null;
    }

    static bool IsTruthy(object? obj)
    {
        if (obj is null) return false;
        if (obj is bool b) return b;

        Console.WriteLine(obj.GetType());
        return true;
    }
}
