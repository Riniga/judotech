using System;
using System.Collections.Generic;

public sealed class Logger
{
    private static readonly Lazy<Logger> lazy = new Lazy<Logger>(() => new Logger());
    public static Logger Instance { get { return lazy.Value; } }
    private Logger() { }

    public void Log(string text)
    {
        Console.WriteLine(DateTime.Now.ToShortTimeString() + " | " + text);
    }
}