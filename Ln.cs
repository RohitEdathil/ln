using System.Text.Json;

class Ln
{
    static ErrorHandler errorHandler = new();

    public static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            StartREPL();
        }
        else if (args.Length == 1)
        {
            RunScript(args[0]);
        }
        else
        {
            Console.WriteLine("Usage: Ln [script_path]");
        }
    }

    static void RunScript(String path)
    {
        try
        {
            string code = File.ReadAllText(path);
            Run(code);

            // Indicate an error in the exit code.
            if (errorHandler.hadError) Environment.Exit(65);
        }
        catch (Exception e)
        {
            Console.WriteLine("Error reading file: " + e.Message);
            return;
        }
    }

    static void StartREPL()
    {
        // REPL loop
        while (true)
        {
            Console.Write("> ");
            string? code = Console.ReadLine();

            // User pressed Ctrl+C
            if (code == null) break;

            Run(code);
        }

    }

    static void Run(string code)
    {
        Scanner scanner = new(code, errorHandler);
        List<Token> tokens = scanner.ScanTokens();


        foreach (Token token in tokens)
        {
            Console.WriteLine(token);
        }

        Parser parser = new(tokens, errorHandler);
        Expression? expression = parser.Parse();

        Interpreter interpreter = new(errorHandler);
        object? result = interpreter.Interpret(expression);

        Console.WriteLine(result);

    }
}