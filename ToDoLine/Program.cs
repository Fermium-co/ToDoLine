using Bit.Core;
using Bit.Owin;
using Bit.Owin.Implementations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Threading.Tasks;

namespace ToDoLine
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            AssemblyContainer.Current.Init();

            AspNetCoreAppEnvironmentsProvider.Current.Use();

            await CreateHostBuilder(args)
                .Build()
                .RunAsync();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            BitWebHost.CreateWebHost<Startup>(args)
#if DEBUG
                .ConfigureWebHostDefaults(webHostBuilder =>
                {
                    webHostBuilder.UseUrls("http://*:53200/");
                })
#endif
            ;
    }
}
