using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ExceptionsDemo
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1. Skapa och konfigurera host-applikationsbyggaren
            HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

            // 2. Registrera egna tjänster i DI-containern
            builder.Services.AddSingleton<IFileProcessorService, FileProcessorService>();
            builder.Services.AddTransient<MainApplication>();

            // 3. Bygg host-containern
            using IHost host = builder.Build();

            // 4. Hämta instansen av startklassen
            var app = host.Services.GetRequiredService<MainApplication>();
            app.Run();
        }
    }
}