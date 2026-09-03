using Microsoft.Extensions.Logging;

namespace judotech.testsupport;

/// <summary>
/// <see cref="ILogger"/> that keeps every formatted message so a test can
/// assert on log content — e.g. "no password, hash, or token was ever logged".
/// </summary>
public class RecordingLogger : ILogger
{
    private readonly List<string> _entries = new();
    private readonly object _gate = new();

    public IReadOnlyList<string> Entries
    {
        get { lock (_gate) return _entries.ToList(); }
    }

    /// <summary>All entries joined with newlines — convenient for `Assert.DoesNotContain`.</summary>
    public string AllText
    {
        get { lock (_gate) return string.Join("\n", _entries); }
    }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => null;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(
        LogLevel logLevel,
        EventId eventId,
        TState state,
        Exception? exception,
        Func<TState, Exception?, string> formatter)
    {
        var line = formatter(state, exception);
        lock (_gate) _entries.Add(line);
    }
}

/// <summary><see cref="RecordingLogger"/> usable where <see cref="ILogger{T}"/> is expected.</summary>
public sealed class RecordingLogger<T> : RecordingLogger, ILogger<T>;
