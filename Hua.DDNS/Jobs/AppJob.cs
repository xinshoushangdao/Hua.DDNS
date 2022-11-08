using Hua.DDNS.Common;
using Hua.DDNS.Common.Config;
using Hua.DDNS.Common.Config.Options;
using Hua.DDNS.Common.Http;
using Hua.DDNS.Start;
using Quartz;
using System.Net;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109.Models;
using Hua.DotNet.Code.Extension;
using TencentCloud.Common;
using TencentCloud.Common.Profile;
using TencentCloud.Dnspod.V20210323;
using TencentCloud.Dnspod.V20210323.Models;

using Tea;
using Tea.Utils;

namespace Hua.DDNS.Jobs
{
    [DisallowConcurrentExecution]
    public class AppJob : IJob, IDisposable
    {
        private readonly ILogger<AppJob> _logger;
        private readonly SettingProvider _settingProvider;
        private readonly DomainOption _domainOption;
        private readonly IHttpHelper _httpHelper;
        public string CurrentIpv4Address;



        public AppJob(ILogger<AppJob> logger,SettingProvider settingProvider, IHttpHelper httpHelper)
        {
            _logger = logger;
            _settingProvider = settingProvider;
            _httpHelper = httpHelper;
            _domainOption = _settingProvider.App.Domain;


        }
        public async Task Execute(IJobExecutionContext context)
        {
            _logger.LogInformation("开始任务执行");
            try
            {
                var oldIp =  (await Dns.GetHostEntryAsync($"{_domainOption.subDomainArray.First()}.{_domainOption.domain}")).AddressList.First();
                CurrentIpv4Address = await _httpHelper.GetCurrentPublicIpv4();

                if (CurrentIpv4Address!=oldIp.ToString())
                {
                   await UpdateDns();
                }
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

        private async Task UpdateDns()
        {
            //更新Ip记录
            switch (_domainOption.Platform)
            {
                case "Tencent":
                    var _dnspodClient = new DnspodClient(
                        // 实例化一个认证对象，入参需要传入腾讯云账户secretId，secretKey,此处还需注意密钥对的保密
                        // 密钥可前往https://console.cloud.tencent.com/cam/capi网站进行获取
                        new Credential { SecretId = _domainOption.Id, SecretKey = _domainOption.Key },
                        "",
                        // 实例化一个client选项，可选的，没有特殊需求可以跳过
                        new ClientProfile()
                        {
                            // 实例化一个http选项，可选的，没有特殊需求可以跳过
                            HttpProfile = new HttpProfile { Endpoint = ("dnspod.tencentcloudapi.com") }
                        });

                    //获取域名解析记录
                    var describeRecordList = await _dnspodClient.DescribeRecordList(new DescribeRecordListRequest() { Domain = _domainOption.domain });
                    var record = describeRecordList.RecordList.FirstOrDefault(m =>
                        m.Value == CurrentIpv4Address && _domainOption.subDomainArray.Any(n => m.Name == n));
                    if (record!=null && record.Value == CurrentIpv4Address) return;//如果记录已经变更，不调用更新接口

                    await _dnspodClient.ModifyRecordBatch(new ModifyRecordBatchRequest()
                    {
                        RecordIdList =
                            describeRecordList.RecordList
                                .Where(m => m.Value != CurrentIpv4Address && _domainOption.subDomainArray.Any(n => m.Name == n))
                                .Select(m => m.RecordId)
                                .ToArray(),
                        Change = "value",
                        ChangeTo = CurrentIpv4Address
                    });

                    break;
                case "Ali":
                    var aliClient =  new AlibabaCloud.SDK.Alidns20150109.Client(new Config()
                    {
                        // 您的 AccessKey ID
                        AccessKeyId = _domainOption.Id,
                        // 您的 AccessKey Secret
                        AccessKeySecret = _domainOption.Key,
                        Endpoint = "alidns.cn-beijing.aliyuncs.com",
                    });

                    var aliDescribeRecordList = (await aliClient.DescribeDomainRecordsAsync(new DescribeDomainRecordsRequest()
                    {
                        DomainName = _domainOption.domain
                    })).Body.DomainRecords.Record; 

                    foreach (var aliDomainRecord in aliDescribeRecordList
                                 .Where(m => m.Value != CurrentIpv4Address && _domainOption.subDomainArray.Any(n => m.RR == n)))
                    {

                        await aliClient.UpdateDomainRecordAsync(new UpdateDomainRecordRequest()
                        {
                            RecordId = aliDomainRecord.RecordId,
                            RR = aliDomainRecord.RR,
                            Type = aliDomainRecord.Type,
                            Value = CurrentIpv4Address,
                        });
                    }
                    break;
            }
        }

        public void Dispose()
        {
            _logger.LogInformation("AppJob已销毁");
        }
    }
}