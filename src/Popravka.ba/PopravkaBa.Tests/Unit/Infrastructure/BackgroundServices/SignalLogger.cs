using Microsoft.Extensions.Logging;

namespace PopravkaBa.Tests.Unit.Infrastructure.BackgroundServices;

internal sealed class SignalLogger<T> : ILogger<T>
{
    private readonly TaskCompletionSource<LogLevel> _logged = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public Task<LogLevel> Logged => _logged.Task;
    public string? Message { get; private set; }
    public Exception? Exception { get; private set; }

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => NullScope.Instance;

    public bool IsEnabled(LogLevel logLevel) => true;

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        Message = formatter(state, exception);
        Exception = exception;
        _logged.TrySetResult(logLevel);
    }

    private sealed class NullScope : IDisposable
    {
        public static NullScope Instance { get; } = new();
        public void Dispose() { }
    }
}
