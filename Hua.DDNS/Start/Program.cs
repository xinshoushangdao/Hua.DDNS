using Hua.DDNS.Common;
using Hua.DDNS.Common.Config;
using Hua.DDNS.Common.Http;
using Hua.DDNS.Jobs;
using Quartz;
using Serilog;

namespace Hua.DDNS.Start
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            await CreateHostBuilder(args).Build().RunAsync();
        }

        private static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .UseSerilog()
                .ConfigureServices((hostContext, services) =>
                {
                    Log.Logger = new LoggerConfiguration()
                        .Enrich.FromLogContext()
                        .WriteTo.Console()
                        .WriteTo.File(
                            Path.Combine("Log\\log-.log"),
                            rollingInterval: RollingInterval.Day)
                        .CreateLogger();
                    ConfigDi(hostContext, services);
                    ConfigQuartz(hostContext, services);
                });

        public static IServiceProvider ConfigDi(HostBuilderContext hostContext, IServiceCollection services)
        {
            services.AddSingleton<SettingProvider>();
            //services.AddSingleton<SyncECMFileProvider>();
            services.AddSingleton<Url>();
            services.AddSingleton<SqlHelper>();
            services.AddTransient<IHttpHelper, HttpHelper>();
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
                var appJobKey = new JobKey("AppJob", "AppJobGroup");
                q.AddJob<AppJob>(j => j
                    .StoreDurably()
                    .WithIdentity(appJobKey)
                    .WithDescription("AppJob")
                );

                q.AddTrigger(t => t
                    .WithIdentity("AppJob Trigger")
                    .ForJob(appJobKey)
                    .WithCronSchedule(hostContext.Configuration.GetSection("App:AppJob:Corn").Value)
                    .WithDescription("AppJob trigger")
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