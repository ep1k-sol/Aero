namespace Aero;

partial class Evaluator
{

    static readonly Random _random = new();

    void RegisterStdLib()
    {
        var math = new Dictionary<string, AeroValue>
        {
            { "pi", new AeroValue(AeroType.NumberValue, Math.PI) },
            { "e", new AeroValue(AeroType.NumberValue, Math.E) },

            { "sin", new AeroValue(AeroType.StdFuncValue, new StdFunction("sin", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Sin(args[0].number))))},
            { "cos", new AeroValue(AeroType.StdFuncValue, new StdFunction("cos", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Cos(args[0].number))))},
            { "tan", new AeroValue(AeroType.StdFuncValue, new StdFunction("tan", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Tan(args[0].number))))},
            { "asin", new AeroValue(AeroType.StdFuncValue, new StdFunction("asin", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Asin(args[0].number))))},
            { "acos", new AeroValue(AeroType.StdFuncValue, new StdFunction("acos", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Acos(args[0].number))))},
            { "atan", new AeroValue(AeroType.StdFuncValue, new StdFunction("atan", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Atan(args[0].number))))},

            { "floor", new AeroValue(AeroType.StdFuncValue, new StdFunction("floor", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Floor(args[0].number)))) },
            { "ceil", new AeroValue(AeroType.StdFuncValue, new StdFunction("ceil", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Ceiling(args[0].number)))) },
            { "sqrt", new AeroValue(AeroType.StdFuncValue, new StdFunction("sqrt", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Sqrt(args[0].number)))) },
            { "abs", new AeroValue(AeroType.StdFuncValue, new StdFunction("abs", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Abs(args[0].number)))) },
            { "round", new AeroValue(AeroType.StdFuncValue, new StdFunction("round", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Round(args[0].number)))) },
            { "min", new AeroValue(AeroType.StdFuncValue, new StdFunction("min", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Min(args[0].number, args[1].number)))) },
            { "max", new AeroValue(AeroType.StdFuncValue, new StdFunction("max", (t, args) =>
                new AeroValue(AeroType.NumberValue, Math.Max(args[0].number, args[1].number)))) },
            { "random", new AeroValue(AeroType.StdFuncValue, new StdFunction("random", (t, args) =>
                new AeroValue(AeroType.NumberValue, _random.NextDouble()))) },
            { "rad", new AeroValue(AeroType.StdFuncValue, new StdFunction("rad", (t, args) =>
                new AeroValue(AeroType.NumberValue, args[0].number * Math.PI / 180.0))) },
            { "angle", new AeroValue(AeroType.StdFuncValue, new StdFunction("angle", (t, args) =>
                new AeroValue(AeroType.NumberValue, args[0].number * 180.0 / Math.PI))) },
        };

        globalEnv.TryDeclare("math", new AeroValue(AeroType.DictValue, math));
    }
}
