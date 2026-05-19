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

    public object? TryGetValue(string name)
    {
        if (body.TryGetValue(name, out var value))
        {
            return value;
        }
        
        return parent?.TryGetValue(name);
    }
}
