using System;

using Microsoft.EntityFrameworkCore;// nuget Microsoft.AspNetCore.All
using Microsoft.Extensions.Configuration;//nuget 包下载安装 Microsoft.AspNetCore.All
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;//nuget 包下载安装 Microsoft.AspNetCore.All
using Logger.File;

namespace ConsoleAppLog
{
    class Program
    {
        static IServiceProvider sp;

        static void Main(string[] args)
        {
            var appsettings = new ConfigurationBuilder()
                .SetBasePath(System.IO.Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            ServiceCollection sc = new ServiceCollection();
            sc.AddLogging();

            sc.AddDbContext<DbContext>(ServiceLifetime.Transient);

            sp = sc.BuildServiceProvider();
            
            sp.GetService<ILoggerFactory>()
                .AddConsole()
                .AddDebug()
                .AddFile(appsettings.GetSection("FileLogging"));
            ILogger _logger = sp.GetService<ILogger<Program>>();
            _logger.LogDebug("开始1-",null);
            _logger.LogInformation("开始2-", new object[0]);
            _logger.LogError("错误-", new object[0]);
            _logger.LogTrace("Trace-", new object[0]);
            _logger.LogWarning("Warning-", new object[0]);
            _logger.LogCritical("Critical 临界-", new object[0]);
            Console.WriteLine("Hello World!");
            Console.ReadLine();
        }
    }
}
