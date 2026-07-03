namespace Aero;

class EnvironmentTable
{
    public Dictionary<string, AeroValue> body = new();
    public EnvironmentTable? parent;
    public EnvironmentTable()
    {
        this.parent = null;
    }

    public EnvironmentTable(EnvironmentTable parent)
    {
        this.parent = parent;
    }

    public bool UpdateValue(string name, AeroValue value)
    {
        if (body.ContainsKey(name))
        {
            body[name] = value;
            return true;
        }

        if (parent != null)
        {
            return parent.UpdateValue(name, value);
        }

        return false;
    }

    public bool TryDeclare(string name, AeroValue value)
    {
        if (body.ContainsKey(name)) return false;
        body.Add(name, value);
        return true;
    }

    public bool TryGetValue(string name, out AeroValue v)
    {
        if (body.TryGetValue(name, out var value))
        {
            v = value;
            return true;
        }

        if (parent != null)
        {
            return parent.TryGetValue(name, out v);
        }

        v = AeroValue.NilValue();
        return false;
    }
}
