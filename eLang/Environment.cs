using eLang.AST;

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

    public bool TryGetValue(string name, out object? v)
    {
        if (body.TryGetValue(name, out var value))
        {
            v = value;
            return true;
        }

        v = null;
        return false;
    }
}
