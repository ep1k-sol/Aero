namespace eLang;

static class Keywords
{
    public static readonly Dictionary<string, TokenType> keywords = new()
    {
        {"and", TokenType.AND },
        {"or", TokenType.OR },
        {"true", TokenType.TRUE },
        {"false", TokenType.FALSE },
        {"if", TokenType.IF },
        {"nil", TokenType.NIL },
        {"local", TokenType.LOCAL },
        {"global", TokenType.GLOBAL },
        {"return", TokenType.RETURN },
        {"while", TokenType.WHILE },
        {"for", TokenType.FOR },
        {"func", TokenType.FUNCTION },
        {"print", TokenType.PRINT },
        {"input", TokenType.INPUT },
    };
}
