using Hua.DDNS.Common;
using Hua.DDNS.Common.Config;
using Hua.DDNS.Common.Config.Options;
using Hua.DDNS.Common.Http;
using Hua.DDNS.Start;
using Quartz;
using System.Net;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109.Models;
using Hua.DDNS.DDNSProviders;
using Hua.DDNS.DDNSProviders.Ali;
using Hua.DDNS.DDNSProviders.Dnspod;
using Hua.DDNS.DDNSProviders.Namesilo;
using Hua.DDNS.Models;
using Hua.DotNet.Code.Extension;
using Microsoft.Extensions.Options;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

using Tea;
using Tea.Utils;

namespace Hua.DDNS.Jobs
{
    [DisallowConcurrentExecution]
    public class NewJob : IJob, IDisposable
    {
        private readonly ILogger<NewJob> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly DdnsOption _ddnsOption;
        private readonly IHttpHelper _httpHelper;
        public string newIp;



        public NewJob(ILogger<NewJob> logger,IHttpHelper httpHelper,IOptions<DdnsOption> ddnsOption, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _httpHelper = httpHelper;
            _serviceProvider = serviceProvider;
            _ddnsOption = ddnsOption.Value;
        }
        public async Task Execute(IJobExecutionContext context)
        {


            try
            {   
                //1. 获取当前机器ip
                var domain = $"{_ddnsOption.SubDomainArray.First()}.{_ddnsOption.Domain}";
                var oldIp = (await Dns.GetHostEntryAsync(domain)).AddressList.First();
                newIp = await _httpHelper.GetCurrentPublicIpv4();
                //1.1 如果当前dns记录与实际dns记录一致，跳出本次执行
                if (newIp == oldIp.ToString()) return;

                //2.获取DNS记录
                IDdnsProvider? ddnsProvider = _ddnsOption.Platform switch
                {
                    PlatformEnum.Namesilo => _serviceProvider.GetRequiredService<NamesiloDdnsProvider>(),
                    PlatformEnum.Tencent => _serviceProvider.GetRequiredService<DnspodDdnsProvider>(),
                    PlatformEnum.Ali => _serviceProvider.GetRequiredService<AliDdnsProvider>(),
                    _ => null
                };

                var dnsRecordList = await ddnsProvider!.GetRecordListAsync();
                var record = dnsRecordList.FirstOrDefault(m => m.Ip == newIp && _ddnsOption.SubDomainArray.Any(n => m.SubDomain == n));
                if (record != null && record.Ip == newIp) return;//如果记录已经变更，不调用更新接口

                //3.比较并更新
                await ddnsProvider.ModifyRecordListAsync(newIp, dnsRecordList.Where(m => m.Ip != newIp && _ddnsOption.SubDomainArray.Any(n => m.SubDomain == n)));
            }
            catch (Exception e)
            {
                _logger.LogError(e,e.Message);
            }
            finally
            {
                _logger.LogInformation("任务执行完成");
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("AppJob已销毁");
        }
    }
}