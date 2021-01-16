using Microsoft.Extensions.Configuration;
using System.IO;

namespace ToDoLine.Util
{
    public static class ToDoLineConfigurationProvider
    {
        public static IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile("appsettings.Development.json", optional: true)
                .Build();
        }
    }
}
