using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;


namespace ExceptionsDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Skapa och konfigurera host-applikationsbyggaren
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            // register custom formatter and select it
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options => options.FormatterName = "MinimalistText")
                .AddConsoleFormatter<MinimalistTextFormatter, ConsoleFormatterOptions>();

            //  Registrera egna tjänster i DI-containern
            builder.Services.AddSingleton<IFileProcessorService, FileProcessorService>();
            builder.Services.AddTransient<MainApplication>();

            // Bygg host-containern
            using IHost host = builder.Build();

            // Hämta instansen av startklassen
            var app = host.Services.GetRequiredService<MainApplication>();
            app.Run();
        }
    }
}

