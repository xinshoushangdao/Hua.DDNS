//using Hua.DDNS.Common;
//using Hua.DDNS.Common.Config;
//using Hua.DDNS.Common.Config.Options;
//using Hua.DDNS.Common.Http;
//using Hua.DDNS.Start;
//using Quartz;
//using System.Net;
//using AlibabaCloud.OpenApiClient.Models;
//using AlibabaCloud.SDK.Alidns20150109.Models;
//using Hua.DotNet.Code.Extension;
//using TencentCloud.Common;
//using TencentCloud.Common.Profile;
//using TencentCloud.Dnspod.V20210323;
//using TencentCloud.Dnspod.V20210323.Models;

//using Tea;
//using Tea.Utils;
//using Hua.DDNS.DDNSProviders;

//namespace Hua.DDNS.Jobs
//{
//    [DisallowConcurrentExecution]
//    public class AppJob : IJob, IDisposable
//    {
//        private readonly ILogger<AppJob> _logger;
//        private readonly SettingProvider _settingProvider;
//        private readonly DdnsOption _ddnsOption;
//        private readonly IHttpHelper _httpHelper;
//        public string CurrentIpv4Address;



//        public AppJob(ILogger<AppJob> logger,SettingProvider settingProvider, IHttpHelper httpHelper)
//        {
//            _logger = logger;
//            _settingProvider = settingProvider;
//            _httpHelper = httpHelper;
//            _ddnsOption = _settingProvider.App.DDNS;


//        }
//        public async Task Execute(IJobExecutionContext context)
//        {
//            _logger.LogInformation("开始任务执行");
//            try
//            {
//                var oldIp =  (await Dns.GetHostEntryAsync($"{_ddnsOption.SubDomainArray.First()}.{_ddnsOption.Domain}")).AddressList.First();
//                CurrentIpv4Address = await _httpHelper.GetCurrentPublicIpv4();

//                if (CurrentIpv4Address!=oldIp.ToString())
//                {
//                   await UpdateDns();
//                }
//            }
//            catch (Exception e)
//            {
//                _logger.LogError(e,e.Message);
//            }
//            finally
//            {
//                _logger.LogInformation("任务执行完成");
//            }
//        }

//        private async Task UpdateDns()
//        {
//            //更新Ip记录
//            switch (_ddnsOption.Platform)
//            {
//                case PlatformEnum.Namesilo:
//                case PlatformEnum.Tencent:
//                    var _dnspodClient = new DnspodClient(
//                        // 实例化一个认证对象，入参需要传入腾讯云账户secretId，secretKey,此处还需注意密钥对的保密
//                        // 密钥可前往https://console.cloud.tencent.com/cam/capi网站进行获取
//                        new Credential { SecretId = _ddnsOption.Id, SecretKey = _ddnsOption.Key },
//                        "",
//                        // 实例化一个client选项，可选的，没有特殊需求可以跳过
//                        new ClientProfile()
//                        {
//                            // 实例化一个http选项，可选的，没有特殊需求可以跳过
//                            HttpProfile = new HttpProfile { Endpoint = ("dnspod.tencentcloudapi.com") }
//                        });

//                    //获取域名解析记录
//                    var describeRecordList = await _dnspodClient.DescribeRecordList(new DescribeRecordListRequest() { Domain = _ddnsOption.Domain });
//                    var record = describeRecordList.RecordList.FirstOrDefault(m =>
//                        m.Value == CurrentIpv4Address && _ddnsOption.SubDomainArray.Any(n => m.Name == n));
//                    if (record!=null && record.Value == CurrentIpv4Address) return;//如果记录已经变更，不调用更新接口

//                    await _dnspodClient.ModifyRecordBatch(new ModifyRecordBatchRequest()
//                    {
//                        RecordIdList =
//                            describeRecordList.RecordList
//                                .Where(m => m.Value != CurrentIpv4Address && _ddnsOption.SubDomainArray.Any(n => m.Name == n))
//                                .Select(m => m.RecordId)
//                                .ToArray(),
//                        Change = "value",
//                        ChangeTo = CurrentIpv4Address
//                    });

//                    break;
//                case PlatformEnum.Ali:
//                    var aliClient =  new AlibabaCloud.SDK.Alidns20150109.Client(new Config()
//                    {
//                        // 您的 AccessKey ID
//                        AccessKeyId = _ddnsOption.Id,
//                        // 您的 AccessKey Secret
//                        AccessKeySecret = _ddnsOption.Key,
//                        Endpoint = "alidns.cn-beijing.aliyuncs.com",
//                    });

//                    var aliDescribeRecordList = (await aliClient.DescribeDomainRecordsAsync(new DescribeDomainRecordsRequest()
//                    {
//                        DomainName = _ddnsOption.Domain
//                    })).Body.DomainRecords.Record; 

//                    foreach (var aliDomainRecord in aliDescribeRecordList
//                                 .Where(m => m.Value != CurrentIpv4Address && _ddnsOption.SubDomainArray.Any(n => m.RR == n)))
//                    {
//                        await aliClient.UpdateDomainRecordAsync(new UpdateDomainRecordRequest()
//                        {
//                            RecordId = aliDomainRecord.RecordId,
//                            RR = aliDomainRecord.RR,
//                            Type = aliDomainRecord.Type,
//                            Value = CurrentIpv4Address,
//                        });
//                        _logger.LogInformation($"Update SubDomain[{aliDomainRecord.RR}.{aliDomainRecord.DomainName}] Value {aliDomainRecord.Value} To {CurrentIpv4Address}");
//                    }
//                    break;
//            }
//        }

//        public void Dispose()
//        {
//            _logger.LogInformation("AppJob已销毁");
//        }
//    }
//}