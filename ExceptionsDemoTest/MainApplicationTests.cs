using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Xunit;
using ExceptionsDemo;

namespace ExceptionsDemo.Tests
{
    // Simple test logger that records messages and levels for assertions
    internal class TestLogger<T> : ILogger<T>
    {
        public List<(LogLevel Level, string Message)> Logs { get; } = new();

        public IDisposable BeginScope<TState>(TState state) => NullScope.Instance;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        {
            var message = formatter(state, exception);
            if (exception != null)
                message += $" Exception: {exception.Message}";
            Logs.Add((logLevel, message));
        }

        private class NullScope : IDisposable
        {
            public static readonly NullScope Instance = new();
            public void Dispose() { }
        }
    }

    // Delegating fake service to control behavior in tests
    internal class DelegatingFileProcessorService : IFileProcessorService
    {
        private readonly Func<string, double> _func;
        public DelegatingFileProcessorService(Func<string, double> func) => _func = func;
        public double ProcessFile(string fileName) => _func(fileName);
    }

    public class MainApplicationTests
    {
        [Fact]
        public void Run_LogsResult_WhenProcessingSucceeds()
        {
            var logger = new TestLogger<MainApplication>();
            var service = new DelegatingFileProcessorService(_ => 25.0); // returned result will be logged
            var app = new MainApplication(service, logger);

            app.Run();

            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Information && l.Message.Contains("Resultat: 25"));
            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Information && l.Message.Contains("Rensning: loggning avslutad."));
            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Information && l.Message.Contains("Programmet avslutas normalt."));
        }

        [Fact]
        public void Run_LogsFileNotFound_WhenServiceThrowsFileNotFoundException()
        {
            var logger = new TestLogger<MainApplication>();
            var service = new DelegatingFileProcessorService(_ => throw new System.IO.FileNotFoundException("numbers.txt not found"));
            var app = new MainApplication(service, logger);

            app.Run();

            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Error && l.Message.Contains("Filen hittades inte:") && l.Message.Contains("numbers.txt not found"));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Rensning: loggning avslutad."));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Programmet avslutas normalt."));
        }

        [Fact]
        public void Run_LogsFormatError_WhenServiceThrowsFormatException()
        {
            var logger = new TestLogger<MainApplication>();
            var service = new DelegatingFileProcessorService(_ => throw new FormatException("Invalid number format"));
            var app = new MainApplication(service, logger);

            app.Run();

            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Error && l.Message.Contains("Formatfel:") && l.Message.Contains("Invalid number format"));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Rensning: loggning avslutad."));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Programmet avslutas normalt."));
        }

        [Fact]
        public void Run_LogsDivideByZero_WhenServiceThrowsDivideByZeroException()
        {
            var logger = new TestLogger<MainApplication>();
            var service = new DelegatingFileProcessorService(_ => throw new DivideByZeroException("Division by zero in file"));
            var app = new MainApplication(service, logger);

            app.Run();

            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Error && l.Message.Contains("Kan inte dividera med noll:") && l.Message.Contains("Division by zero in file"));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Rensning: loggning avslutad."));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Programmet avslutas normalt."));
        }

        [Fact]
        public void Run_LogsUnknownError_WhenServiceThrowsGenericException()
        {
            var logger = new TestLogger<MainApplication>();
            var service = new DelegatingFileProcessorService(_ => throw new Exception("Something went wrong"));
            var app = new MainApplication(service, logger);

            app.Run();

            Assert.Contains(logger.Logs, l => l.Level == LogLevel.Error && l.Message.Contains("Okänt fel:") && l.Message.Contains("Something went wrong"));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Rensning: loggning avslutad."));
            Assert.Contains(logger.Logs, l => l.Message.Contains("Programmet avslutas normalt."));
        }
    }
}