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
                    .AddEnvironmentVariables()// �ѻ�������Ҳ�ŵ� Configuraiton����
                    .Build();

                var sc = DIConfig.ConfigureServices(config);
                var job = sc.GetService<AppJob>();

                job?.Execute(null);
            }
            catch (Exception e)
            {
                Assert.False(false, $"�����쳣:{e.Message}");
            }
        }
    }
}