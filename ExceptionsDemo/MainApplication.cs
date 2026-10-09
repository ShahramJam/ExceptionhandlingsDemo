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
            logger.LogInformation("=== Start av programmet ===");

            try
            {
                logger.LogInformation("Försöker läsa fil och räkna...");
                var path = Path.Combine(AppContext.BaseDirectory, "numbers.txt");
                var result = fileProcessorService.ProcessFile(path);

                logger.LogInformation($"\nResultat: {result}");
            }
            catch (FileNotFoundException ex)
            {
                logger.LogError($"Filen hittades inte: {ex.Message}");
            }
            catch (FormatException ex)
            {
                logger.LogError($"Formatfel: {ex.Message}");
            }
            catch (DivideByZeroException ex)
            {
                logger.LogError($"Kan inte dividera med noll: {ex.Message}");
            }
            catch (Exception ex)
            {
                logger.LogError($"Okänt fel: {ex.Message}");
            }
            finally
            {
                // Rensning som alltid körs, även vid undantag
                logger.LogInformation("Rensning: loggning avslutad.");
            }

            logger.LogInformation("Programmet avslutas normalt.");

        }
    }
}