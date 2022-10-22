using Hua.DDNS.Jobs;
using Hua.DDNS.Test.Start;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace Hua.DDNS.Test
{
    public class AppJobTest
    {
        [Theory]
        [InlineData("appsetting.Ali.json")]
        [InlineData("appsetting.Tencent.json")]
        public void UpdateDNS(string configPath)
        {
            try
            {
                var config = new ConfigurationBuilder()
                    .SetBasePath(AppContext.BaseDirectory)
                    .AddJsonFile(configPath, true)
                    .AddEnvironmentVariables()// 把环境变量也放到 Configuraiton当中
                    .Build();

                var sc = DIConfig.ConfigureServices(config);
                var job = sc.GetService<AppJob>();

                job?.Execute(null);
            }
            catch (Exception e)
            {
                Assert.False(false, $"请求异常:{e.Message}");
            }
        }
    }
}