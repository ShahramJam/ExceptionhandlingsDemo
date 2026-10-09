using Microsoft.Extensions.Logging;
using System;
using System.IO;

namespace ExceptionsDemo
{
    // Huvudapplikationsklass som körs via DI
    public class MainApplication(IFileProcessorService fileProcessorService, ILogger<MainApplication> logger)
    {
        // Kör programmets logik
        public void Run()
        {
            logger.LogInformation("Konsolapplikationen har startat.");

            Console.WriteLine("=== Start av programmet ===");

            try
            {
                Console.WriteLine("Försöker läsa fil och räkna...");
                var path = Path.Combine(AppContext.BaseDirectory, "numbers.txt");
                var result = fileProcessorService.ProcessFile(path);

                Console.WriteLine($"\nResultat: {result}");
            }
            catch (FileNotFoundException ex)
            {
                Console.WriteLine($"Filen hittades inte: {ex.Message}");
            }
            catch (FormatException ex)
            {
                Console.WriteLine($"Formatfel: {ex.Message}");
            }
            catch (DivideByZeroException ex)
            {
                Console.WriteLine($"Kan inte dividera med noll: {ex.Message}");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Okänt fel: {ex.Message}");
            }
            finally
            {
                // Rensning som alltid körs, även vid undantag
                Console.WriteLine("Rensning: loggning avslutad.");
            }

            Console.WriteLine("Programmet avslutas normalt.");

            logger.LogInformation("Konsolapplikationen avslutade framgångsrikt.");
        }
    }
}