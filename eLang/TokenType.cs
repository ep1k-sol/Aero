namespace eLang;

enum TokenType
{
    // block indicator
    LEFT_PAREN, RIGHT_PAREN,
    LEFT_BRACKET, RIGHT_BRACKET,
    LEFT_BRACE, RIGHT_BRACE,

    // punctuation
    DOT,
    COMMA,
    SEMICOLON,

    // operator
    MODULO,
    POWER,

    PLUS, PLUS_EQUAL,
    MINUS, MINUS_EQUAL,
    STAR, STAR_EQUAL,
    SLASH, SLASH_EQUAL,
    EQUAL, EQUAL_EQUAL,
    BANG, BANG_EQUAL,
    GREATER, GREATER_EQUAL,
    LESS, LESS_EQUAL,

    // literal
    IDENTIFIER,
    STRING,
    NUMBER,

    // keyword
    AND, OR,
    TRUE, FALSE,
    IF, ELSE,
    NIL,
    LOCAL, GLOBAL,
    RETURN,
    WHILE,
    FOR,
    FUNCTION,
    PRINT,

    EOF
}