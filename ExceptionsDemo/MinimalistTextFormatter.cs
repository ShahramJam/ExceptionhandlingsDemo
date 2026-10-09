using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using System.IO;

namespace ExceptionsDemo
{
    // A simple console formatter that omits the category/class name.
    public class MinimalistTextFormatter : ConsoleFormatter
    {
        public MinimalistTextFormatter() : base("MinimalistText") { }

        public override void Write<TState>(in LogEntry<TState> logEntry, IExternalScopeProvider? scopeProvider, TextWriter textWriter)
        {
            // Get your actual message text
            string message = logEntry.Formatter(logEntry.State, logEntry.Exception);
            textWriter.WriteLine($"{logEntry.LogLevel}: {message}");
        }
    }
}
