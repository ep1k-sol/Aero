using eLang.AST;
using System.ComponentModel.DataAnnotations;

namespace eLang;

class EnvironmentTable
{
    public Dictionary<string, object?> body = new();
    public EnvironmentTable? parent;
    public EnvironmentTable()
    {
        this.parent = null;
    }

    public EnvironmentTable(EnvironmentTable parent)
    {
        this.parent = parent;
    }

    public bool UpdateValue(string name, object? value)
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

    public bool TryGetValue(string name, out object? v)
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

        v = null;
        return false;
    }
}
