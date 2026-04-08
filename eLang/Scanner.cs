using System.Data.Common;

namespace eLang;

class Scanner
{
    readonly string _source;
    readonly List<Token> _tokens = new List<Token>();

    private int start = 0;
    private int current = 0;
    private int line = 1;

    public Scanner(string source)
    {
        this._source = source;
    }

    public List<Token> ScanTokens()
    {
        while (!IsAtEnd())
        {
            start = current;
            ScanToken();
        }

        _tokens.Add(new Token(TokenType.EOF, "", null, line));
        return _tokens;
    }

    void ScanToken()
    {
        char c = Advance();
        switch (c)
        {
            // single character
            case '(': AddToken(TokenType.LEFT_PAREN); break;
            case ')': AddToken(TokenType.RIGHT_PAREN); break;
            case '[': AddToken(TokenType.LEFT_BRACKET); break;
            case ']': AddToken(TokenType.RIGHT_BRACKET); break;
            case '{': AddToken(TokenType.LEFT_BRACE); break;
            case '}': AddToken(TokenType.RIGHT_BRACE); break;
            case '.': AddToken(TokenType.DOT); break;
            case ',': AddToken(TokenType.COMMA); break;
            case ';': AddToken(TokenType.SEMICOLON); break;
            case '^': AddToken(TokenType.POWER); break;
            case '%': AddToken(TokenType.MODULO); break;

            // two characters
            case '+': AddToken(Match('=') ? TokenType.PLUS_EQUAL : TokenType.PLUS); break;
            case '-': AddToken(Match('=') ? TokenType.MINUS_EQUAL : TokenType.MINUS); break;
            case '*': AddToken(Match('=') ? TokenType.STAR_EQUAL : TokenType.STAR); break;
            case '=': AddToken(Match('=') ? TokenType.EQUAL_EQUAL : TokenType.EQUAL); break;
            case '!': AddToken(Match('=') ? TokenType.BANG_EQUAL : TokenType.BANG); break;
            case '>': AddToken(Match('=') ? TokenType.GREATER_EQUAL : TokenType.GREATER); break;
            case '<': AddToken(Match('=') ? TokenType.LESS_EQUAL : TokenType.LESS); break;

            case '/':
                if (Match('/'))
                    while (!IsAtEnd() && CheckNext() != '\n') Advance();
                else
                    AddToken(Match('=') ? TokenType.SLASH_EQUAL : TokenType.SLASH);
                break;

            case '"': String('"'); break;
            case '\'': String('\''); break;

            case ' ': break;
            case '\r': break;
            case '\t': break;
            case '\n': line++; break;




            default:
                if (IsDigit(c))
                    Number();
                else if (IsAlpha(c))
                    Identifier();
                else

                    Program.Error(line, Errors.UNEXPECTED_CHAR);
                break;
        }
    }

    TokenType MatchKeywordOrIdentifier(string literal)
    {
        if (Keywords.keywords.TryGetValue(literal, out var keyword))
        {
            return keyword;
        }
        else
        {
            return TokenType.IDENTIFIER;
        }
    }

    void Number()
    {
        while (IsDigit(CheckNext())) Advance();

        if (CheckNext() == '.' && IsDigit(CheckNextNext()))
        {
            Advance();

            while (IsDigit(CheckNext())) Advance();
        }

        AddToken(TokenType.NUMBER, double.Parse(_source.Substring(start, current - start)));
    }

    void String(char c)
    {
        while (CheckNext() != c && !IsAtEnd())
        {
            if (CheckNext() == '\n') line++;
            Advance();
        }

        if (IsAtEnd())
        {
            Program.Error(line, Errors.UNTERMINATED_STRING);
            return;
        }

        Advance();

        AddToken(TokenType.STRING, _source.Substring(start + 1, current - start - 2));
    }

    void Identifier()
    {
        while (IsAlphaOrDigit(CheckNext()) && !IsAtEnd())
        {
            Advance();
        }

        string literal = _source.Substring(start, current - start);

        AddToken(MatchKeywordOrIdentifier(literal), literal);
    }



    bool IsDigit(char c) => (c >= '0' && c <= '9');

    bool IsAlpha(char c)
    {
        uint v = (uint)(c | 0x20); // 와 나 이거 진짜 개천재다
        return (v >= 'a' && v <= 'z');
    }

    bool IsAlphaOrDigit(char c) => (IsDigit(c) || IsAlpha(c));

    bool Match(char expected)
    {
        if (IsAtEnd()) return false;
        if (_source[current] != expected) return false;

        current++;
        return true;
    }

    char CheckNext()
    {
        if (IsAtEnd()) return '\0';

        return _source[current];
    }

    char CheckNextNext()
    {
        if (current + 1 >= _source.Length) return '\0';

        return _source[current + 1];
    }

    bool IsAtEnd()
    {
        return current >= _source.Length;
    }

    char Advance()
    {
        return _source[current++];
    }

    void AddToken(TokenType type)
    {
        AddToken(type, null);
    }

    void AddToken(TokenType type, object? literal)
    {
        string text = _source.Substring(start, current - start);
        _tokens.Add(new Token(type, text, literal, line));
    }
}