namespace eLang;

public static class Errors
{
    // scanner
    public const string UNEXPECTED_CHAR = "Unexpected Character.";
    public const string UNTERMINATED_STRING = "Unterminated String";

    // parser
    public const string UNTERMINATED_PARENTHESIS = "Expect ')' after Expression or Function Call.";
    public const string MISSING_PARENTHESIS = "'(' is missing.";
    public const string UNTERMINATED_BRACE = "Expect '}' after idk what to call it.";
    public const string MISSING_BRACE = "'{' is missing.";


    public const string IDENTIFIER = "Identifier is missing or wrong.";
    public const string MISSING_PUNCUATION = "',' is missing.";
    public const string MISSING_EQUAL = "Expect '=' after Identifier.";

    public const string UNKNOWN = "UNKNOWN ERROR.";
}