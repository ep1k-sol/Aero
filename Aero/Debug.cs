using Aero.AST;

namespace Aero;

static class Debug
{
    public static void PrintTokens(List<Token> tokens)
    {
        foreach (Token token in tokens)
        {
            Console.WriteLine(token.ToString());
        }
    }

    public static void PrintAST(List<Stmt> ast)
    {
        Console.WriteLine("\nAST result\n");
        PrintStmts(ast);
    }

    static void PrintStmts(List<Stmt> ast, int depth = 0)
    {
        foreach (Stmt stmt in ast)
        {
            string indent = new string(' ', depth * 2);

            if (stmt is ExprStmt e)
            {
                PrintExprAST(e.expr, depth);
            }

            if (stmt is Print p)
            {
                Console.WriteLine($"{indent}PRINT (");
                foreach (Expr expr in p.value)
                {
                    PrintExprAST(expr, depth + 2);
                }
                Console.WriteLine($"{indent})");
            }

            if (stmt is Variable v)
            {
                Console.WriteLine($"{indent}VARIABLE: {v.scope.lexeme}, {v.name}, VALUE: [");
                PrintExprAST(v.value, depth + 2);
                Console.WriteLine($"{indent}]");
            }

            if (stmt is Function f)
            {
                Console.WriteLine($"{indent}FUNCTION: [");
                Console.WriteLine($"{indent}  SCOPE: [ {f.scope.lexeme} ]");
                Console.WriteLine($"{indent}  NAME: [ {f.name.lexeme} ]");
                Console.WriteLine($"{indent}  PARAMETERS: [");
                foreach (Token id in f.param)
                {
                    Console.WriteLine($"{indent}    {id.lexeme}");
                }
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}  CODE: [");
                PrintStmts(f.block.code, depth + 2);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}]");
            }

            if (stmt is If i)
            {
                Console.WriteLine($"{indent}IF: [");
                Console.WriteLine($"{indent}  CONDITION: [");
                PrintExprAST(i.condition, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}  BODY: [");
                PrintStmts(i.block.code, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}]");
            }

            if (stmt is While w)
            {
                Console.WriteLine($"{indent}WHILE: [");
                Console.WriteLine($"{indent}  CONDITION: [");
                PrintExprAST(w.condition, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}  BODY: [");
                PrintStmts(w.block.code, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}]");
            }

            if (stmt is For fr)
            {
                Console.WriteLine($"{indent}FOR: [");
                Console.WriteLine($"{indent}  INIT: [");
                PrintStmts(new List<Stmt> { fr.init }, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}  CONDITION: [");
                PrintExprAST(fr.condition, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}  STEP: [");
                PrintExprAST(fr.step, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}  BODY: [");
                PrintStmts(fr.block.code, depth + 4);
                Console.WriteLine($"{indent}  ]");
                Console.WriteLine($"{indent}]");
            }

            if (stmt is Return r)
            {
                Console.WriteLine($"{indent}RETURN: [");
                PrintExprAST(r.value, depth + 2);
                Console.WriteLine($"{indent}]");
            }

            if (stmt is Break br)
            {
                Console.WriteLine($"{indent}BREAK");
            }

            if (stmt is Block b)
            {
                Console.WriteLine($"{indent}BLOCK: [");
                PrintStmts(b.code, depth + 2);
                Console.WriteLine($"{indent}]");
            }
        }
    }

    static void PrintExprAST(Expr? expr, int depth = 0)
    {
        if (expr is null) return;
        string indent = new string(' ', depth * 2);

        if (expr is Literal l)
        {
            Console.WriteLine($"{indent}LITERAL: {l.value}");
            return;
        }

        if (expr is VariableExpr v)
        {
            Console.WriteLine($"{indent}VARIABLE_EXPR: {v.value.lexeme}");
            return;
        }

        if (expr is Assign a)
        {
            Console.WriteLine($"{indent}ASSIGN [");
            Console.WriteLine($"{indent}  target [");
            PrintExprAST(a.target, depth + 2);
            Console.WriteLine($"{indent}  value [");
            PrintExprAST(a.value, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is Lambda lm)
        {
            Console.WriteLine($"{indent}LAMBDA [");
            Console.WriteLine($"{indent}  PARAMETERS: [");
            foreach (Token id in lm.param)
            {
                Console.WriteLine($"{indent}    {id.lexeme}");
            }
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  CODE: [");
            PrintStmts(lm.block.code, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
        }

        if (expr is Call c)
        {
            Console.WriteLine($"{indent}CALL: {c.callee}()");
            return;
        }

        if (expr is Binary b)
        {
            Console.WriteLine($"{indent}BINARY [");
            Console.WriteLine($"{indent}  left [");
            PrintExprAST(b.left, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  op [ {b.op.lexeme} ]");
            Console.WriteLine($"{indent}  right [");
            PrintExprAST(b.right, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is Unary u)
        {
            Console.WriteLine($"{indent}UNARY [");
            Console.WriteLine($"{indent}  op [ {u.op.lexeme} ]");
            Console.WriteLine($"{indent}  right [");
            PrintExprAST(u.right, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is Postfix p)
        {
            Console.WriteLine($"{indent}POSTFIX [");
            Console.WriteLine($"{indent}  left [");
            PrintExprAST(p.left, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  op [ {p.op.lexeme} ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is Group g)
        {
            Console.WriteLine($"{indent}GROUP (");
            PrintExprAST(g.paren, depth + 2);
            Console.WriteLine($"{indent})");
            return;
        }

        if (expr is ArrayLiteral arr)
        {
            Console.WriteLine($"{indent}ARRAY [");
            foreach (var element in arr.elements)
            {
                PrintExprAST(element, depth + 2);
            }
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is IndexExpr ie)
        {
            Console.WriteLine($"{indent}INDEX [");
            Console.WriteLine($"{indent}  target [");
            PrintExprAST(ie.target, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  index [");
            PrintExprAST(ie.index, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is IndexAssign ia)
        {
            Console.WriteLine($"{indent}INDEX_ASSIGN [");
            Console.WriteLine($"{indent}  target [");
            PrintExprAST(ia.target, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  index [");
            PrintExprAST(ia.index, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  value [");
            PrintExprAST(ia.value, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is DictLiteral dl)
        {
            Console.WriteLine($"{indent}DICT {{");
            foreach (var (key, value) in dl.pairs)
            {
                Console.WriteLine($"{indent}  {key.lexeme}: [");
                PrintExprAST(value, depth + 2);
                Console.WriteLine($"{indent}  ]");
            }
            Console.WriteLine($"{indent}}}");
            return;
        }

        if (expr is FieldExpr fe)
        {
            Console.WriteLine($"{indent}FIELD [");
            Console.WriteLine($"{indent}  target [");
            PrintExprAST(fe.target, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  name [ {fe.name.lexeme} ]");
            Console.WriteLine($"{indent}]");
            return;
        }

        if (expr is FieldAssign fa)
        {
            Console.WriteLine($"{indent}FIELD_ASSIGN [");
            Console.WriteLine($"{indent}  target [");
            PrintExprAST(fa.target, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}  name [ {fa.name.lexeme} ]");
            Console.WriteLine($"{indent}  value [");
            PrintExprAST(fa.value, depth + 2);
            Console.WriteLine($"{indent}  ]");
            Console.WriteLine($"{indent}]");
            return;
        }
    }

    public static void PrintEvaluated(object? result)
    {
        Console.WriteLine(result);
    }
}