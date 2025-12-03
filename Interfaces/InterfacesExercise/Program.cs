using System;

ILogger c = new ConsoleLogger();
c.Log("Testing Console Logger");

ILogger f = new FileLogger();
f.Log("Testing File Logger");

DatabaseLogger dbConcrete = new DatabaseLogger();
((ILogger)dbConcrete).Log("Testing Database Logger");

public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"Console: {message}");
}

public class FileLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"File (simulated): {message}");
    }
}

public class DatabaseLogger : ILogger
{
    void ILogger.Log(string message)
    {
        Console.WriteLine($"DB (simulated insert): {message}");
    }

    public void BulkInsert(string[] messages) =>
        Console.WriteLine($"Bulk insert {messages.Length} messages");
}
