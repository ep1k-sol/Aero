
namespace eLang;

class Evaluator
{
    static public object? Evaluate(Expr expr)
    {
        if (expr is Literal l)
        {
            return l.value;
        }

        if (expr is Binary b)
        {
            var left = Evaluate(b.left);
            var right = Evaluate(b.right);

            if (left is null || right is null) return null;

            switch (b.op.type)
            {
                case TokenType.PLUS: return (double)left + (double)right;
                case TokenType.MINUS: return (double)left - (double)right;
                case TokenType.STAR: return (double)left * (double)right;
                case TokenType.SLASH: return (double)left / (double)right;
                case TokenType.POWER: return (Math.Pow((double)left, (double)right));
                case TokenType.MODULO: return (double)left % (double)right;
            }
        }

        if (expr is Unary u)
        {
            var right = Evaluate(u.right);

            if (right is null) return null;

            switch (u.op.type)
            {
                case TokenType.PLUS: return (double)right;
                case TokenType.MINUS: return -(double)right;
            }
        }

        if (expr is Group g)
        {
            return Evaluate(g);
        }


        return null;
    }
}
