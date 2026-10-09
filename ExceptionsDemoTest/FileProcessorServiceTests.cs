using System;
using System.IO;
using ExceptionsDemo;
using Xunit;

namespace ExceptionsDemoTest
{
    public class FileProcessorServiceTests
    {
        [Fact]
        public void ProcessFile_ValidNumber_ReturnsExpected()
        {
            var file = Path.GetTempFileName();
            try
            {
                File.WriteAllText(file, "25");
                var svc = new FileProcessorService();
                double result = svc.ProcessFile(file);
                Assert.Equal(4.0, result, 6);
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ProcessFile_NullOrWhitespaceFileName_ThrowsArgumentException(string fileName)
        {
            var svc = new FileProcessorService();
            Assert.Throws<ArgumentException>(() => svc.ProcessFile(fileName!));
        }

        [Fact]
        public void ProcessFile_FileNotFound_ThrowsFileNotFoundException()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N") + ".txt");
            if (File.Exists(path)) File.Delete(path);

            var svc = new FileProcessorService();
            Assert.Throws<FileNotFoundException>(() => svc.ProcessFile(path));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void ProcessFile_EmptyOrWhitespaceContent_ThrowsInvalidOperationException(string content)
        {
            var file = Path.GetTempFileName();
            try
            {
                File.WriteAllText(file, content);
                var svc = new FileProcessorService();
                Assert.Throws<InvalidOperationException>(() => svc.ProcessFile(file));
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Fact]
        public void ProcessFile_NonIntegerContent_ThrowsFormatException()
        {
            var file = Path.GetTempFileName();
            try
            {
                File.WriteAllText(file, "abc");
                var svc = new FileProcessorService();
                Assert.Throws<FormatException>(() => svc.ProcessFile(file));
            }
            finally
            {
                File.Delete(file);
            }
        }

        [Fact]
        public void ProcessFile_Zero_ThrowsDivideByZeroException()
        {
            var file = Path.GetTempFileName();
            try
            {
                File.WriteAllText(file, "0");
                var svc = new FileProcessorService();
                Assert.Throws<DivideByZeroException>(() => svc.ProcessFile(file));
            }
            finally
            {
                File.Delete(file);
            }
        }
    }
}