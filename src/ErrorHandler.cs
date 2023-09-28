
class ErrorHandler
{
    public bool hadError = false;
    public void Error(int line, string message)
    {
        hadError = true;
        Report(line, "", message);
    }

    public void Report(int line, string where, string message)
    {
        Console.WriteLine("[line " + line + "] Error" + where + ": " + message);
    }
}