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

            try
            {
                switch (b.op.type)
                {
                    case TokenType.PLUS: return Convert.ToDouble(left) + Convert.ToDouble(right);
                    case TokenType.MINUS: return Convert.ToDouble(left) - Convert.ToDouble(right);
                    case TokenType.STAR: return Convert.ToDouble(left) * Convert.ToDouble(right);
                    case TokenType.SLASH: return Convert.ToDouble(left) / Convert.ToDouble(right);
                    case TokenType.POWER: return Math.Pow(Convert.ToDouble(left), Convert.ToDouble(right));
                    case TokenType.MODULO: return Convert.ToDouble(left) % Convert.ToDouble(right);

                    case TokenType.GREATER_EQUAL: return Convert.ToDouble(left) >= Convert.ToDouble(right);
                    case TokenType.GREATER: return Convert.ToDouble(left) > Convert.ToDouble(right);
                    case TokenType.LESS_EQUAL: return Convert.ToDouble(left) <= Convert.ToDouble(right);
                    case TokenType.LESS: return Convert.ToDouble(left) < Convert.ToDouble(right);

                    case TokenType.BANG_EQUAL: return !Equals(left, right);
                    case TokenType.EQUAL_EQUAL: return Equals(left, right);

                    case TokenType.DOTDOT: return $"{left}{right}";
                }
            }
            catch (FormatException)
            {
                throw new Exception("Invalid literal for numeric operation");
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
