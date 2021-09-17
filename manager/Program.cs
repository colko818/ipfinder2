using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Manager
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Run on all interfaces, port 5000/5001
                    webBuilder.UseStartup<Startup>()
                        .UseUrls(urls: "https://*:5001;http://*:5000");
                });
    }
}
