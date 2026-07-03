namespace Aero;

static class TypeMethods
{
    public static AeroValue GetField(AeroValue target, string name, Token token)
    {
        if (name == "type")
            return new AeroValue(AeroType.StringValue, target.type.Name());

        return target.type switch
        {
            AeroType.StringValue => GetStringField(target, name, token),
            AeroType.ArrayValue => GetArrayField(target, name, token),
            AeroType.DictValue => GetDictField(target, name, token),
            _ => throw new NameError(token, $"'{target.type.Name()}' has no attribute '{name}'.")
        };
    }

    static AeroValue GetStringField(AeroValue target, string name, Token token)
    {
        return name switch
        {
            "length" => new AeroValue(AeroType.NumberValue, (double)target.String.Length),
            _ => throw new NameError(token, $"'string' has no attribute '{name}'.")
        };
    }

    static AeroValue GetArrayField(AeroValue target, string name, Token token)
    {
        var list = target.Array;

        return name switch
        {
            "length" => new AeroValue(AeroType.NumberValue, (double)list.Count),

            "push" => new AeroValue(AeroType.StdFuncValue, new StdFunction("push", (t, args) => {
                list.Add(args[0]);
                return AeroValue.NilValue();
            })),

            "pop" => new AeroValue(AeroType.StdFuncValue, new StdFunction("pop", (t, args) => {
                if (list.Count == 0)
                    throw new IndexError(t, "Cannot pop from empty array.");
                var last = list[^1];
                list.RemoveAt(list.Count - 1);
                return last;
            })),

            _ => throw new NameError(token, $"'array' has no attribute '{name}'.")
        };
    }

    static AeroValue GetDictField(AeroValue target, string name, Token token)
    {
        var dict = target.Dict;

        // built-in methods
        if (name == "setAlternative")
            return new AeroValue(AeroType.StdFuncValue, new StdFunction("setAlternative", (t, args) => {
                if (args.Count != 1 || args[0].type != AeroType.DictValue)
                    throw new TypeError(t, "setAlternative() requires a single dictionary argument.");
                dict["__alt__"] = args[0];
                return args[0];
            }));

        if (name == "keys")
            return new AeroValue(AeroType.StdFuncValue, new StdFunction("keys", (t, args) => {
                var keys = dict.Keys
                    .Where(k => k != "__alt__")
                    .Select(k => new AeroValue(AeroType.StringValue, k))
                    .ToList();
                return new AeroValue(AeroType.ArrayValue, keys);
            }));

        // user-defined fields + __alt__ chain
        return DictGet(dict, name, token);
    }

    static AeroValue DictGet(Dictionary<string, AeroValue> dict, string key, Token token)
    {
        if (dict.TryGetValue(key, out var value))
            return value;

        if (dict.TryGetValue("__alt__", out var alt) && alt.type == AeroType.DictValue)
            return DictGet(alt.Dict, key, token);

        throw new NameError(token, $"'dictionary' has no attribute '{key}'.");
    }
}