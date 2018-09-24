using Bit.OwinCore;
using Microsoft.AspNetCore.Hosting;
using System.Threading.Tasks;

namespace ToDoLine
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateWebHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            BitWebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
