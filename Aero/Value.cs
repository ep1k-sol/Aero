namespace Aero;

enum AeroType
{
    BoolValue,
    NilValue,
    NumberValue,
    StringValue,
    ArrayValue,
    DictValue,
    FuncValue,
    StdFuncValue,
}

struct AeroValue
{
    public AeroType type;
    public double number;
    public bool boolean;
    public object? obj;

    public AeroValue(AeroType type, object? obj)
    {
        this.type = type;
        this.obj = obj;
    }

    public AeroValue(AeroType type, double number)
    {
        this.type = type;
        this.number = number;
    }

    public AeroValue(AeroType type, bool boolean)
    {
        this.type = type;
        this.boolean = boolean;
    }

    public string String => (string)obj!;
    public List<AeroValue> Array => (List<AeroValue>)obj!;
    public Dictionary<string, AeroValue> Dict => (Dictionary<string, AeroValue>)obj!;
    public AeroFunction func => (AeroFunction)obj!;
    public StdFunction stdfunc => (StdFunction)obj!;



    public static AeroValue NilValue() => new(AeroType.NilValue, (object?)null);

    public override string ToString()
    {
        return type switch
        {
            AeroType.NumberValue => number % 1 == 0 ? ((int)number).ToString() : number.ToString(),
            AeroType.BoolValue => boolean.ToString(),
            AeroType.NilValue => "nil",
            AeroType.StringValue => String,
            AeroType.ArrayValue => $"[{string.Join(", ", Array.Select(e => e.ToString()))}]",
            AeroType.DictValue => $"{{{string.Join(", ", Dict.Select(p => $"{p.Key}: {p.Value}"))}}}",
            AeroType.FuncValue => "<function>",
            AeroType.StdFuncValue => "<stdfunction>",
            _ => ""
        };
    }
}

static class AeroTypeExtensions
{
    public static string Name(this AeroType type) => type switch
    {
        AeroType.NumberValue => "number",
        AeroType.StringValue => "string",
        AeroType.BoolValue => "bool",
        AeroType.NilValue => "nil",
        AeroType.ArrayValue => "array",
        AeroType.DictValue => "dictionary",
        AeroType.FuncValue => "function",
        AeroType.StdFuncValue => "stdfunction",
        _ => "unknown"
    };
}