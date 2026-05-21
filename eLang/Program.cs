using eLang.AST;

namespace eLang;

class Program
{
    static bool hadError = false;

    public static void Main(string[] args)
    {
        if (args.Length > 1)
        {
            Console.WriteLine("Usage: eLang [path to script]");
            Environment.Exit(1);
        }
        else if (args.Length == 1)
        {
            RunFile(args[0]);
        }
        else
        {
            RunPrompt();
        }
    }

    static void RunFile(string path)
    {
        string source;
        try
        {
            source = File.ReadAllText(path);
        }
        catch (Exception)
        {
            Console.WriteLine($"Cannot open file '{path}': No such file or directory");
            return;
        }

        Run(source);

        if (hadError) Environment.Exit(2);
    }

    static void RunPrompt()
    {
        while (true)
        {
            Console.Write("> ");
            string? line = Console.ReadLine();

            if (line == null) break;
            Run(line);

            hadError = false;
        }
    }

    static void Run(string source)
    {
        try
        {
            Scanner scanner = new Scanner(source);
            List<Token> tokens = scanner.ScanTokens();

            Debug.PrintTokens(tokens);

            Parser parser = new Parser(tokens);
            List<Stmt> ast = parser.Parse();

            Debug.PrintAST(ast);

            Evaluator evaluator = new Evaluator();

            evaluator.Evaluate(ast);
            // Debug.PrintEvaluated(result);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error occured: {e.Message}");
            hadError = true;
        }
    }

    public static void Error(ushort line, string message, ushort column)
    {
        Report(line, message, $"column {column}");
    }

    public static void Error(Token token, string message)
    {
        if (token.type == TokenType.EOF)
        {
            Report(token.line, $"{message} got {token.lexeme}.", $"column {token.column} at end");
        }
        else
        {
            Report(token.line, $"{message} got '{token.lexeme}'.", $"column {token.column}");
        }
    }

    static void Report(int line, string message, string where)
    {
        Console.Error.WriteLine($"[line {line}] {message} Error at near {where}");
        hadError = true;
    }
}