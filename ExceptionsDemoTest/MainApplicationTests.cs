using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ExceptionsDemo;

namespace ExceptionsDemoTest
{
    // Simple test doubles for IFileProcessorService to avoid external mocking libraries.
    class SuccessfulFileProcessor : IFileProcessorService
    {
        private readonly double _result;
        public SuccessfulFileProcessor(double result) => _result = result;
        public double ProcessFile(string fileName) => _result;
    }

    class ThrowingFileProcessor : IFileProcessorService
    {
        private readonly Exception _ex;
        public ThrowingFileProcessor(Exception ex) => _ex = ex;
        public double ProcessFile(string fileName) => throw _ex;
    }

    public class MainApplicationTests
    {
        private static string CaptureConsoleOutput(Action action)
        {
            var originalOut = Console.Out;
            try
            {
                using var sw = new StringWriter();
                Console.SetOut(sw);
                action();
                Console.Out.Flush();
                return sw.ToString();
            }
            finally
            {
                Console.SetOut(originalOut);
            }
        }

        [Fact]
        public void Run_WhenProcessFileSucceeds_PrintsResultAndNormalFlowMessages()
        {
            var service = new SuccessfulFileProcessor(25.0);
            var logger = NullLogger<MainApplication>.Instance;
            var app = new MainApplication(service, logger);

            var output = CaptureConsoleOutput(() => app.Run());

            Assert.Contains("=== Start av programmet ===", output);
            Assert.Contains("Försöker läsa fil och räkna...", output);
            Assert.Contains("Resultat: 25", output); // formatted without decimals may appear as "25" or "25,0"; check major substring
            Assert.Contains("Rensning: loggning avslutad.", output);
            Assert.Contains("Programmet avslutas normalt.", output);
        }

        [Fact]
        public void Run_WhenFileNotFound_PrintsFileNotFoundMessage()
        {
            var service = new ThrowingFileProcessor(new FileNotFoundException("numbers.txt saknas"));
            var logger = NullLogger<MainApplication>.Instance;
            var app = new MainApplication(service, logger);

            var output = CaptureConsoleOutput(() => app.Run());

            Assert.Contains("Filen hittades inte", output);
            Assert.Contains("Rensning: loggning avslutad.", output);
        }

        [Fact]
        public void Run_WhenFormatException_PrintsFormatErrorMessage()
        {
            var service = new ThrowingFileProcessor(new FormatException("ogiltigt tal"));
            var logger = NullLogger<MainApplication>.Instance;
            var app = new MainApplication(service, logger);

            var output = CaptureConsoleOutput(() => app.Run());

            Assert.Contains("Formatfel", output);
            Assert.Contains("Rensning: loggning avslutad.", output);
        }

        [Fact]
        public void Run_WhenDivideByZero_PrintsDivideByZeroMessage()
        {
            var service = new ThrowingFileProcessor(new DivideByZeroException("division med noll"));
            var logger = NullLogger<MainApplication>.Instance;
            var app = new MainApplication(service, logger);

            var output = CaptureConsoleOutput(() => app.Run());

            Assert.Contains("Kan inte dividera med noll", output);
            Assert.Contains("Rensning: loggning avslutad.", output);
        }

        [Fact]
        public void Run_WhenGenericException_PrintsUnknownErrorMessage()
        {
            var service = new ThrowingFileProcessor(new Exception("något gick fel"));
            var logger = NullLogger<MainApplication>.Instance;
            var app = new MainApplication(service, logger);

            var output = CaptureConsoleOutput(() => app.Run());

            Assert.Contains("Okänt fel", output);
            Assert.Contains("Rensning: loggning avslutad.", output);
        }
    }
}