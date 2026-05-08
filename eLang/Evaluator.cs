using eLang.AST;
using System.Xml.Schema;

namespace eLang;

class Evaluator
{
    static public void Evaluate(List<Stmt> stmts)
    {
        foreach (Stmt stmt in stmts)
        {
            if (stmt is ExprStmt e)
            {
                EvaluateExpr(e.expr);
            }

            if (stmt is Print p)
            {
                var texts = new List<string?>();

                foreach (Expr v in p.value)
                {
                    var value = EvaluateExpr(v);
                    texts.Add(value?.ToString());
                }

                Console.WriteLine(string.Join('\t', texts));
            }
        }
    }

    static private object? EvaluateExpr(Expr expr)
    {
        if (expr is Literal l)
        {
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

            left = Convert.ToDouble(left);
            right = Convert.ToDouble(right);

            switch (b.op.type)
            {
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

            if (right is null) return null;

            switch (u.op.type)
            {
                case TokenType.PLUS: return (double)right;
                case TokenType.MINUS: return -(double)right;
            }
        }

        if (expr is Group g)
        {
            return EvaluateExpr(g.paren);
        }


        return null;
    }
}
