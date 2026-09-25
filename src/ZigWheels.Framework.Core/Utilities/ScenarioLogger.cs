namespace ZigWheels.Framework.Core.Utilities;

/// <summary>
/// Writes a dedicated log file for a single automated test scenario.
///
/// Each scenario receives its own logger so that test executions remain
/// isolated and the logging design remains suitable for parallel testing.
/// </summary>
public sealed class ScenarioLogger : IDisposable
{
    private readonly StreamWriter _writer;
    private readonly object _syncLock = new();
    private bool _disposed;

    public string FilePath { get; }

    /// <summary>
    /// Creates a scenario log using the shared artefact folder convention.
    /// </summary>
    public ScenarioLogger(
        string framework,
        string scenarioName)
    {
        FilePath =
            ArtifactPaths.CreateLogPath(
                framework,
                scenarioName);

        _writer =
            new StreamWriter(
                FilePath,
                append: true)
            {
                AutoFlush = true
            };
    }

    /// <summary>
    /// Writes an informational log entry.
    /// </summary>
    public void Info(
        string message)
    {
        Write(
            "INFO",
            message);
    }

    /// <summary>
    /// Writes a warning log entry.
    /// </summary>
    public void Warning(
        string message)
    {
        Write(
            "WARN",
            message);
    }

    /// <summary>
    /// Writes an error log entry.
    /// </summary>
    public void Error(
        string message)
    {
        Write(
            "ERROR",
            message);
    }

    /// <summary>
    /// Writes an exception to the scenario log.
    /// </summary>
    public void Error(
        string message,
        Exception exception)
    {
        Write(
            "ERROR",
            $"{message}{Environment.NewLine}" +
            $"{exception.GetType().Name}: {exception.Message}{Environment.NewLine}" +
            $"{exception.StackTrace}");
    }

    private void Write(
        string level,
        string message)
    {
        if (_disposed)
        {
            return;
        }

        var timestamp =
            DateTime.Now.ToString(
                "yyyy-MM-dd HH:mm:ss.fff");

        lock (_syncLock)
        {
            _writer.WriteLine(
                $"{timestamp} [{level}] {message}");
        }
    }

    /// <summary>
    /// Flushes and closes the scenario log.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        lock (_syncLock)
        {
            if (_disposed)
            {
                return;
            }

            _writer.Flush();
            _writer.Dispose();

            _disposed = true;
        }
    }
}