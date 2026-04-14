namespace eLang;

public static class Errors
{
    // scanner
    public const string UNEXPECTED_CHAR = "Unexpected Character.";
    public const string UNTERMINATED_STRING = "Unterminated String";

    // parser
    public const string UNTERMINATED_PARENTHESIS = "')' is missing.";

    public const string IDENTIFIER = "Identifier is missing or wrong.";
    public const string MISSING_EQUAL = "'=' is missing.";
}