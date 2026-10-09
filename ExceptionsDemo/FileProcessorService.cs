using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ExceptionsDemo
{
    public interface IFileProcessorService
    {
        double ProcessFile(string fileName);
    }

    public class FileProcessorService(ILogger<FileProcessorService> logger) : IFileProcessorService
    {

        // Exempel på metod som själv kastar ett undantag (throw)
        public double ProcessFile(string fileName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName))
                    throw new ArgumentException("Filnamn får inte vara tomt eller null.", nameof(fileName));

                // Let StreamReader throw FileNotFoundException if the file is missing
                using var reader = new StreamReader(fileName);

                string? line = reader.ReadLine();
                if (string.IsNullOrWhiteSpace(line))
                    throw new InvalidOperationException("Filen är tom eller innehåller inga giltiga rader.");

                line = line.Trim();
                if (!int.TryParse(line, out int number))
                    throw new FormatException($"Kan inte tolka '{line}' som ett heltal.");

                if (number == 0)
                    throw new DivideByZeroException("Det första talet i filen är noll, division med noll.");

                return 100.0 / number;
            }
            catch (ArgumentException ex)
            {
                logger.LogError($"Argumentfel i ProcessFile: {ex.Message}");
                throw;
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError($"Filen hittades inte i ProcessFile: {ex.Message}");
                throw;
            }
            catch (FormatException ex)
            {
                logger.LogError($"Formatfel i ProcessFile: {ex.Message}");
                throw;
            }
            catch (DivideByZeroException ex)
            {
                logger.LogError($"Nolldivision i ProcessFile: {ex.Message}");
                throw;
            }
            catch (InvalidOperationException ex)
            {
                logger.LogError($"Ogiltigt tillstånd i ProcessFile: {ex.Message}");
                throw;
            }
            catch (IOException ex)
            {
                logger.LogError($"IO-fel i ProcessFile: {ex.Message}");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError($"Okänt fel i ProcessFile: {ex.Message}");
                throw;
            }
        }
    }
}