namespace eLang;

readonly struct Token
{
    public readonly TokenType type;
    public readonly string lexeme;
    public readonly object? literal;
    public readonly ushort line;
    public readonly ushort column;

    public Token(TokenType type, string lexeme, object? literal, ushort line, ushort column)
    {
        this.type = type;
        this.lexeme = lexeme;
        this.literal = literal;
        this.line = line;
        this.column = column;
    }

    public override string ToString()
    {
        return $"{type} {lexeme} {literal}";
    }
}