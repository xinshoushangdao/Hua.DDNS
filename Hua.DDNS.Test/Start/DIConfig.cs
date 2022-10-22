using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hua.DDNS.Common.Config;
using Hua.DDNS.Common.Http;
using Hua.DDNS.Common;
using Hua.DDNS.Jobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;

namespace Hua.DDNS.Test.Start
{
    public class DIConfig
    {
        public static IServiceProvider ConfigureServices(IConfiguration configuration)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine("Log\\log-.log"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddLogging(loggingBuilder =>
            {
                loggingBuilder.AddConfiguration(configuration.GetSection("Logging")); //配置logging的一些东西
                // 下面的这行需要 Microsoft.Extensions.Logging.Console
                loggingBuilder.AddConsole(); //加多个 每一个Ilooger下面就会有多个provider
            });

            // 注入了一个默认的ILogger
            services.AddSingleton<SettingProvider>();
            //services.AddSingleton<SyncECMFileProvider>();
            services.AddSingleton<Url>();
            services.AddSingleton<SqlHelper>();
            services.AddTransient<IHttpHelper, HttpHelper>();
            services.AddTransient<AppJob>();
            return services.BuildServiceProvider();
        }
    }
}
