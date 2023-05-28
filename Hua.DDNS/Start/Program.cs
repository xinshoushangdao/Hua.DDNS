using System.Configuration;
using System.Reflection;
using Hua.DDNS.Common;
using Hua.DDNS.Common.Config;
using Hua.DDNS.Common.Http;
using Hua.DDNS.DDNSProviders;
using Hua.DDNS.DDNSProviders.Ali;
using Hua.DDNS.DDNSProviders.Dnspod;
using Hua.DDNS.DDNSProviders.Namesilo;
using Hua.DDNS.Jobs;
using Microsoft.Extensions.DependencyInjection;
using Quartz;
using Serilog;
using Serilog.Extensions.Logging;

namespace Hua.DDNS.Start
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    Path.Combine("Log\\log-.log"),
                    rollingInterval: RollingInterval.Day)
                .CreateLogger();

            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureAppConfiguration((context, config) =>
                {
                    // clear all config provider
                    config.Sources.Clear();
                    config
                        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                        .AddJsonFile($"appsettings.{context.HostingEnvironment.EnvironmentName}.json", optional: true,
                            reloadOnChange: true);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddAutoMapper(Assembly.GetExecutingAssembly());
                    services.Configure<DdnsOption>(hostContext.Configuration.GetSection("DDNS"));
                    services.Configure<NamesiloOption>(hostContext.Configuration.GetSection("Namesilo"));
                    services.Configure<DnspodOption>(hostContext.Configuration.GetSection("Dnspod"));
                    services.Configure<AliDdnsOption>(hostContext.Configuration.GetSection("Ali"));
                    ConfigDi(hostContext, services);
                    ConfigQuartz(hostContext, services);
                });

        public static IServiceProvider ConfigDi(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton<SettingProvider>();
            services.AddSingleton<Url>();
            services.AddSingleton<SqlHelper>();
            services.AddTransient<IHttpHelper, HttpHelper>();
            services.AddTransient<NamesiloDdnsProvider>();
            services.AddTransient<AliDdnsProvider>();
            services.AddTransient<DnspodDdnsProvider>();
            return services.BuildServiceProvider();
        }

        private static void ConfigQuartz(HostBuilderContext hostContext, IServiceCollection services)
        {
            // if you are using persistent job store, you might want to alter some options
            services.Configure<QuartzOptions>(options =>
            {
                options.Scheduling.IgnoreDuplicates = true; // default: false
                options.Scheduling.OverWriteExistingData = true; // default: true
            });

            // base configuration for DI
            services.AddQuartz(q =>
            {
                // handy when part of cluster or you want to otherwise identify ltiple schedulers
                q.SchedulerId = "Hua.DDNS.Demo";
                // this is default configuration if you don't alter it
                q.UseMicrosoftDependencyInjectionJobFactory();

                // these are the defaults
                q.UseSimpleTypeLoader();
                q.UseInMemoryStore();
                q.UseDefaultThreadPool(tp => { tp.MaxConcurrency = 10; });

                //configure jobs with code
                var appJobKey = new JobKey("NewJob", "NewJobGroup");
                q.AddJob<NewJob>(j => j
                    .StoreDurably()
                    .WithIdentity(appJobKey)
                    .WithDescription("NewJob")
                );

                q.AddTrigger(t => t
                    .WithIdentity("NewJob Trigger")
                    .ForJob(appJobKey)
                    .WithCronSchedule(hostContext.Configuration.GetSection("App:AppJob:Corn").Value)
                    .WithDescription("NewJob trigger")
                    .StartNow()
                );
            });

            // Quartz.Extensions.Hosting hosting
            services.AddQuartzHostedService(options =>
            {
                // when shutting down we want jobs to complete gracefully
                options.WaitForJobsToComplete = true;

                // when we need to init another IHostedServices first
                options.StartDelay = TimeSpan.FromSeconds(10);
            });
        }
    }
}