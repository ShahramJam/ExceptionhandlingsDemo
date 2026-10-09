using System;
using System.IO;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;
using ExceptionsDemo;

namespace ExceptionsDemo.Tests
{
    public class FileProcessorServiceTests
    {
        private readonly FileProcessorService _service = new FileProcessorService(NullLogger<FileProcessorService>.Instance);

        [Fact]
        public void ProcessFile_ReturnsExpected_ForValidNumber()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "25");
                var result = _service.ProcessFile(path);
                Assert.Equal(4.0, result, 5);
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void ProcessFile_ThrowsArgumentException_ForNullOrWhiteSpace(string? fileName)
        {
            Assert.Throws<ArgumentException>(() => _service.ProcessFile(fileName!));
        }

        [Fact]
        public void ProcessFile_ThrowsFileNotFoundException_ForMissingFile()
        {
            var path = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + ".txt");
            Assert.False(File.Exists(path));
            Assert.Throws<FileNotFoundException>(() => _service.ProcessFile(path));
        }

        [Fact]
        public void ProcessFile_ThrowsInvalidOperationException_ForEmptyFile()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, string.Empty);
                Assert.Throws<InvalidOperationException>(() => _service.ProcessFile(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ProcessFile_ThrowsFormatException_ForNonInteger()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "abc");
                Assert.Throws<FormatException>(() => _service.ProcessFile(path));
            }
            finally
            {
                File.Delete(path);
            }
        }

        [Fact]
        public void ProcessFile_ThrowsDivideByZeroException_ForZero()
        {
            var path = Path.GetTempFileName();
            try
            {
                File.WriteAllText(path, "0");
                Assert.Throws<DivideByZeroException>(() => _service.ProcessFile(path));
            }
            finally
            {
                File.Delete(path);
            }
        }
    }
}