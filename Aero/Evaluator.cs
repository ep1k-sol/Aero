using Aero.AST;

namespace Aero;

partial class Evaluator
{
    string dirpath;

    EnvironmentTable globalEnv = new EnvironmentTable();
    EnvironmentTable currentEnv;

    public Evaluator(string path)
    {
        this.dirpath = path;
        currentEnv = globalEnv;
        RegisterBuiltIns();
        RegisterStdLib();
    }

    public record EvalResult(AeroValue value, bool isReturn = false, bool isBreak = false)
    {
        public static EvalResult Void => new(AeroValue.NilValue());
    }

    public EvalResult Evaluate(List<Stmt> stmts)
    {
        foreach (Stmt stmt in stmts)
        {
            var result = EvaluateStmt(stmt);
            if (result.isReturn) return result;
            if (result.isBreak) return result;
        }
        return EvalResult.Void;
    }

    public EvalResult EvaluateStmt(Stmt stmt)
    {
        if (stmt is Variable v)
        {
            var name = v.name;
            var value = EvaluateExpr(v.value);
            var env = v.scope.type == TokenType.GLOBAL ? globalEnv : currentEnv;

            if (!env.TryDeclare(name.lexeme, value))
                throw new NameError(name, $"Cannot redeclare variable '{name.lexeme}' in the same scope.");
        }

        if (stmt is Block b)
        {
            var previousEnv = currentEnv;
            currentEnv = new EnvironmentTable(currentEnv);

            var result = Evaluate(b.code);

            currentEnv = previousEnv;
            return result;
        }

        if (stmt is Function f)
        {
            var func = new AeroValue(AeroType.FuncValue, new AeroFunction(f.param, f.block, currentEnv));

            switch (f.scope.type)
            {
                case TokenType.LOCAL: currentEnv.body.Add(f.name.lexeme, func); break;
                case TokenType.GLOBAL: globalEnv.body.Add(f.name.lexeme, func); break;
            }
        }

        if (stmt is If i)
        {
            var condition = EvaluateExpr(i.condition);

            if (IsTruthy(condition))
            {
                var result = EvaluateStmt(i.block);
                if (result.isReturn) return result;
                if (result.isBreak) return result;
            }
            else if (i.elseBranch is not null)
            {
                EvaluateStmt(i.elseBranch);
            }
        }

        if (stmt is While w)
        {
            while (IsTruthy(EvaluateExpr(w.condition)))
            {
                var result = EvaluateStmt(w.block);
                if (result.isReturn) return result;
                if (result.isBreak) break;
            }
        }

        if (stmt is For fr)
        {
            var previousEnv = currentEnv;
            currentEnv = new EnvironmentTable(currentEnv);

            EvaluateStmt(fr.init);

            while (IsTruthy(EvaluateExpr(fr.condition)))
            {
                var result = EvaluateStmt(fr.block);
                if (result.isReturn) return result;
                if (result.isBreak) break;

                EvaluateExpr(fr.step);
            }

            currentEnv = previousEnv;
        }

        if (stmt is Return r)
        {
            AeroValue value = r.value != null ? EvaluateExpr(r.value) : AeroValue.NilValue();
            return new EvalResult(value, isReturn: true);
        }

        if (stmt is Break)
        {
            return new EvalResult(AeroValue.NilValue(), isBreak: true);
        }

        if (stmt is ExprStmt e)
        {
            EvaluateExpr(e.expr);
        }

        return EvalResult.Void;
    }

    private AeroValue EvaluateExpr(Expr? expr)
    {
        if (expr is Literal l)
        {
            switch (l.value)
            {
                case bool bl: return new AeroValue(AeroType.BoolValue, bl);
                case null: return AeroValue.NilValue();
                case string s: return new AeroValue(AeroType.StringValue, s);
                case double d: return new AeroValue(AeroType.NumberValue, d);
            }
        }

        if (expr is VariableExpr v)
        {
            string name = v.value.lexeme;
            if (currentEnv.TryGetValue(name, out var value))
            {
                return value;
            }

            throw new NameError(v.value, $"Undefined variable '{name}'.");
        }

        if (expr is Assign a)
        {
            var value = EvaluateExpr(a.value);

            if (a.target is VariableExpr vxpr)
            {
                if (currentEnv.UpdateValue(vxpr.value.lexeme, value))
                {
                    return value;
                }

                throw new NameError(vxpr.value, $"Undefined variable '{vxpr.value.lexeme}'.");
            }

            throw new TypeError(a.token, "Invalid assignment target.");
        }

        if (expr is Lambda lm)
        {
            var func = new AeroFunction(lm.param, lm.block, currentEnv);

            return new AeroValue(AeroType.FuncValue, func);
        }

        if (expr is Call c)
        {
            var callee = EvaluateExpr(c.callee);
            var args = new List<AeroValue>();

            foreach (var arg in c.args)
            {
                args.Add(EvaluateExpr(arg));
            }

            switch (callee.type)
            {
                case AeroType.StdFuncValue: return callee.stdfunc.Call(c.token, args);
                case AeroType.FuncValue: return CallFunction(callee.func, args);
                default:
                    throw new TypeError(c.token, $"'{callee}' is not a function.");

            }
        }

        if (expr is Binary b)
        {
            if (b.op.type == TokenType.AND)
            {
                var lef = EvaluateExpr(b.left);
                return IsTruthy(lef) ? EvaluateExpr(b.right) : lef;
            }
            if (b.op.type == TokenType.OR)
            {
                var lef = EvaluateExpr(b.left);
                return IsTruthy(lef) ? lef : EvaluateExpr(b.right);
            }

            var left = EvaluateExpr(b.left);
            var right = EvaluateExpr(b.right);

            switch (b.op.type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                case TokenType.STAR:
                case TokenType.POWER:
                case TokenType.MODULO:
                case TokenType.GREATER_EQUAL:
                case TokenType.GREATER:
                case TokenType.LESS_EQUAL:
                case TokenType.LESS:
                    {
                        if (left.type != AeroType.NumberValue || right.type != AeroType.NumberValue)
                        {
                            throw new TypeError(b.op, $"Operator '{b.op.lexeme}' requires numeric operands.");
                        }

                        return b.op.type switch
                        {
                            TokenType.PLUS => new AeroValue(AeroType.NumberValue, left.number + right.number),
                            TokenType.MINUS => new AeroValue(AeroType.NumberValue, left.number - right.number),
                            TokenType.STAR => new AeroValue(AeroType.NumberValue, left.number * right.number),
                            TokenType.POWER => new AeroValue(AeroType.NumberValue, Math.Pow(left.number, right.number)),
                            TokenType.MODULO => new AeroValue(AeroType.NumberValue, left.number % right.number),
                            TokenType.GREATER_EQUAL => new AeroValue(AeroType.BoolValue, left.number >= right.number),
                            TokenType.GREATER => new AeroValue(AeroType.BoolValue, left.number > right.number),
                            TokenType.LESS_EQUAL => new AeroValue(AeroType.BoolValue, left.number <= right.number),
                            TokenType.LESS => new AeroValue(AeroType.BoolValue, left.number < right.number),
                            _ => AeroValue.NilValue()
                        };
                    }

                case TokenType.SLASH:
                    {
                        if (left.type != AeroType.NumberValue || right.type != AeroType.NumberValue)
                            throw new TypeError(b.op, "Operator '/' requires numeric operands.");

                        var divisor = right.number;
                        if (divisor == 0)
                            throw new ZeroDivisionError(b.op, "Division by zero.");

                        return new AeroValue(AeroType.NumberValue, left.number / divisor);
                    }

                case TokenType.BANG_EQUAL: return new AeroValue(AeroType.BoolValue, !AeroEquals(left, right));
                case TokenType.EQUAL_EQUAL: return new AeroValue(AeroType.BoolValue, AeroEquals(left, right));

                case TokenType.DOTDOT: return new AeroValue(AeroType.StringValue, $"{left.ToString()}{right.ToString()}");
            }
        }

        if (expr is Unary u)
        {
            var right = EvaluateExpr(u.right);

            switch (u.op.type)
            {
                case TokenType.PLUS:
                case TokenType.MINUS:
                    {
                        if (right.type != AeroType.NumberValue)
                            throw new TypeError(u.op, $"Operator '{u.op.lexeme}' requires a numeric operand.");

                        return u.op.type == TokenType.PLUS
                            ? new AeroValue(AeroType.NumberValue, right.number)
                            : new AeroValue(AeroType.NumberValue, -right.number);
                    }

                case TokenType.BANG:
                    return new AeroValue(AeroType.BoolValue, !IsTruthy(right));

                case TokenType.PLUS_PLUS:
                case TokenType.MINUS_MINUS:
                    {
                        if (right.type != AeroType.NumberValue)
                            throw new TypeError(u.op, $"Operator '{u.op.lexeme}' requires a numeric operand.");

                        if (u.right is VariableExpr uv)
                        {
                            var newVal = u.op.type == TokenType.PLUS_PLUS
                                ? new AeroValue(AeroType.NumberValue, right.number + 1)
                                : new AeroValue(AeroType.NumberValue, right.number - 1);

                            currentEnv.UpdateValue(uv.value.lexeme, newVal);
                            return newVal;
                        }

                        throw new TypeError(u.op, "Prefix increment target must be a variable.");
                    }
            }
        }

        if (expr is Postfix p)
        {
            if (p.left is VariableExpr pv)
            {
                var current = EvaluateExpr(p.left);

                if (current.type != AeroType.NumberValue)
                    throw new TypeError(p.op, $"Operator '{p.op.lexeme}' requires a numeric operand.");

                var newVal = p.op.type == TokenType.PLUS_PLUS
                    ? new AeroValue(AeroType.NumberValue, current.number + 1)
                    : new AeroValue(AeroType.NumberValue, current.number - 1);

                currentEnv.UpdateValue(pv.value.lexeme, newVal);
                return current;
            }

            throw new TypeError(p.op, "Postfix increment target must be a variable.");
        }

        if (expr is ArrayLiteral arr)
        {
            var elements = new List<AeroValue>();

            foreach (var element in arr.elements)
            {
                elements.Add(EvaluateExpr(element));
            }

            return new AeroValue(AeroType.ArrayValue, elements);
        }

        if (expr is IndexExpr ie)
        {
            var target = EvaluateExpr(ie.target);
            var index = EvaluateExpr(ie.index);

            if (target.type == AeroType.ArrayValue)
            {
                if (index.type != AeroType.NumberValue || index.number % 1 != 0)
                    throw new TypeError(ie.bracket, "Array index must be an integer.");

                var i = (int)index.number;
                if (i < 0 || i >= target.Array.Count)
                    throw new IndexError(ie.bracket, $"Index {i} is out of range (array length: {target.Array.Count}).");

                return target.Array[i];
            }

            if (target.type == AeroType.DictValue)
            {
                if (index.type != AeroType.StringValue)
                    throw new TypeError(ie.bracket, "Dictionary index must be a string.");

                if (!target.Dict.TryGetValue(index.String, out var value))
                    throw new NameError(ie.bracket, $"Undefined key '{index.String}'.");

                return value;
            }

            throw new TypeError(ie.bracket, "Index operator requires an array or dictionary.");
        }

        if (expr is IndexAssign ia)
        {
            var target = EvaluateExpr(ia.target);
            var index = EvaluateExpr(ia.index);
            var value = EvaluateExpr(ia.value);

            if (target.type == AeroType.ArrayValue)
            {
                if (index.type != AeroType.NumberValue || index.number % 1 != 0)
                    throw new TypeError(ia.bracket, "Array index must be an integer.");

                var i = (int)index.number;
                if (i < 0 || i >= target.Array.Count)
                    throw new IndexError(ia.bracket, $"Index {i} is out of range (array length: {target.Array.Count}).");

                target.Array[i] = value;
                return value;
            }

            if (target.type == AeroType.DictValue)
            {
                if (index.type != AeroType.StringValue)
                    throw new TypeError(ia.bracket, "Dictionary index must be a string.");

                target.Dict[index.String] = value;
                return value;
            }

            throw new TypeError(ia.bracket, "Index operator requires an array or dictionary.");
        }

        if (expr is DictLiteral dl)
        {
            var dict = new Dictionary<string, AeroValue>();

            foreach (var (key, value) in dl.pairs)
            {
                var k = key.lexeme;
                var val = EvaluateExpr(value);
                dict[k] = val;
            }

            return new AeroValue(AeroType.DictValue, dict);
        }

        if (expr is FieldExpr fe)
        {
            var target = EvaluateExpr(fe.target);
            return TypeMethods.GetField(target, fe.name.lexeme, fe.name);
        }

        if (expr is FieldAssign fa)
        {
            var target = EvaluateExpr(fa.target);

            if (target.type != AeroType.DictValue)
                throw new TypeError(fa.dot, "Field Assigning access requires a dictionary.");

            var value = EvaluateExpr(fa.value);
            target.Dict[fa.name.lexeme] = value;
            return value;
        }

        if (expr is Group g)
        {
            return EvaluateExpr(g.paren);
        }

        return AeroValue.NilValue();
    }

    // helper
    static bool IsTruthy(AeroValue val)
    {
        if (val.type == AeroType.NilValue) return false;
        if (val.type == AeroType.BoolValue) return val.boolean;

        return true;
    }

    AeroValue CallFunction(AeroFunction func, List<AeroValue> args)
    {
        var env = new EnvironmentTable(func.closure);

        // define param
        for (int idx = 0; idx < func.param.Count; idx++)
        {
            string name = func.param[idx].lexeme;

            var value = idx < args.Count ? args[idx] : AeroValue.NilValue();
            env.TryDeclare(name, value);
        }

        var previousEnv = currentEnv;
        currentEnv = env;

        var result = Evaluate(func.body.code);

        currentEnv = previousEnv;
        return result.value;
    }

    AeroValue RunFile(string pth)
    {
        var source = File.ReadAllText($"/{dirpath}{pth}.aero");

        var tokens = new Scanner(source).ScanTokens();
        var ast = new Parser(tokens).Parse();

        var evaluator = new Evaluator(pth);
        return evaluator.Evaluate(ast).value;
    }

    bool AeroEquals(AeroValue a, AeroValue b)
    {
        if (a.type != b.type) return false;

        return a.type switch
        {
            AeroType.NumberValue => a.number == b.number,
            AeroType.BoolValue => a.boolean == b.boolean,
            AeroType.NilValue => true,
            _ => Equals(a.obj, b.obj)
        };
    }
}
