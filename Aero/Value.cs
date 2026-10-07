using System.Globalization;

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


    public bool TryToNumber(out double result)
    {
        switch (type)
        {
            case AeroType.NumberValue:
                result = number;
                return true;
            case AeroType.BoolValue:
                result = boolean ? 1 : 0;
                return true;
            case AeroType.StringValue:
                return double.TryParse(String, NumberStyles.Float,
                                       CultureInfo.InvariantCulture, out result);
            default:
                result = 0;
                return false;
        }
    }

    public override string ToString()
    {
        return type switch
        {
            AeroType.NumberValue => double.IsFinite(number) && number % 1 == 0 && Math.Abs(number) < 1e15
    ? ((long)number).ToString(CultureInfo.InvariantCulture)
    : number.ToString(CultureInfo.InvariantCulture),
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