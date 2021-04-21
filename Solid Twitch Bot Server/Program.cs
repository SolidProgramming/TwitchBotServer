using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Solid_Twitch_Bot_Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            args = new string[1];

            if (Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") != "true")
            {
                args[0] = "http://[::1]:0;https://[::1]:0";
            }

            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    if (string.IsNullOrEmpty(args[0]))
                    {
                        webBuilder.UseStartup<Startup>();
                    }
                    else
                    {
                        webBuilder.UseStartup<Startup>().UseUrls(args[0]);
                    }
                });
    }
}
