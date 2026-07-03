using Aero.AST;

namespace Aero;

partial class Evaluator
{
    void RegisterBuiltIns()
    {
        Register("toNumber", (t, args) =>
        {
            var val = args[0];
            return val.type switch
            {
                AeroType.NumberValue => val,
                AeroType.StringValue => double.TryParse(val.String, out var num)
                    ? new AeroValue(AeroType.NumberValue, num)
                    : throw new TypeError(t, $"Cannot convert '{val.String}' to a number"),
                _ => throw new TypeError(t, $"Cannot convert {val.type.Name()} to number.")
            };
        });

        Register("toString", (t, args) => new AeroValue(AeroType.StringValue, args[0].ToString()));

        Register("input", (token, args) => {
            if (args.Count > 0 && args[0].type == AeroType.StringValue)
                Console.Write(args[0].String);
            return new AeroValue(AeroType.StringValue, Console.ReadLine());
        });

        Register("print", (token, args) => {
            Console.WriteLine(string.Join("\t", args.Select(a => a.ToString())));
            return AeroValue.NilValue();
        });

        Register("write", (token, args) => {
            Console.Write(string.Join("", args.Select(a => a.ToString())));
            return AeroValue.NilValue();
        });

        Register("import", (t, args) =>
            {
                if (args.Count != 1 || args[0].type != AeroType.StringValue)
                    throw new TypeError(t, "import() requires a single string path argument.");
                return args[0].String switch
                {
                    "io" => new AeroValue(AeroType.DictValue, new Dictionary<string, AeroValue>()),
                    _ => RunFile(args[0].String)
                };
            });
    }

    void Register(string name, Func<Token, List<AeroValue>, AeroValue> func)
    {
        globalEnv.TryDeclare(name, new AeroValue(AeroType.StdFuncValue, new StdFunction(name, func)));
    }
}
