namespace Aero;

static class Keywords
{
    public static readonly Dictionary<string, TokenType> keywords = new()
    {
        {"and", TokenType.AND },
        {"or", TokenType.OR },
        {"true", TokenType.TRUE },
        {"false", TokenType.FALSE },
        {"if", TokenType.IF },
        {"else", TokenType.ELSE },
        {"nil", TokenType.NIL },
        {"local", TokenType.LOCAL },
        {"global", TokenType.GLOBAL },
        {"return", TokenType.RETURN },
        {"while", TokenType.WHILE },
        {"break", TokenType.BREAK },
        {"for", TokenType.FOR },
        {"func", TokenType.FUNCTION },
    };
}
