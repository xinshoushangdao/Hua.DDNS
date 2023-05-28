using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Hua.DDNS.DDNSProviders.Ali;
using Hua.DDNS.Models;
using Microsoft.Extensions.Options;
using TencentCloud.Common.Profile;
using TencentCloud.Common;
using TencentCloud.Dnspod.V20210323;
using System.Net;
using TencentCloud.Dnspod.V20210323.Models;

namespace Hua.DDNS.DDNSProviders.Dnspod
{

    /// <summary>
    /// DdnsProvider for Dnspod
    /// </summary>
    public class DnspodDdnsProvider : IDdnsProvider
    {

        private readonly DnspodClient _client;
        private readonly DnspodOption _dnspodOption;
        private readonly DdnsOption _ddnsOption;
        private readonly IMapper _mapper;

        public DnspodDdnsProvider(IMapper mapper, IOptions<DnspodOption> dnspodOption, IOptions<DdnsOption> ddnsOption)
        {
            _mapper = mapper;
            _dnspodOption = dnspodOption.Value;
            _ddnsOption = ddnsOption.Value;

            _client = new DnspodClient(
                // 实例化一个认证对象，入参需要传入腾讯云账户secretId，secretKey,此处还需注意密钥对的保密
                // 密钥可前往https://console.cloud.tencent.com/cam/capi网站进行获取
                new Credential { SecretId = _dnspodOption.Id, SecretKey = _dnspodOption.Key },
                "",
                // 实例化一个client选项，可选的，没有特殊需求可以跳过
                new ClientProfile()
                {
                    // 实例化一个http选项，可选的，没有特殊需求可以跳过
                    HttpProfile = new HttpProfile { Endpoint = (_dnspodOption.Endpoint) }//"dnspod.tencentcloudapi.com"
                });
        }

        public async Task<IEnumerable<DnsRecord>?> GetRecordListAsync()
        {
            var recordList = (await _client.DescribeRecordList(new DescribeRecordListRequest() { Domain = _ddnsOption.Domain })).RecordList;

            return _mapper.Map<IEnumerable<DnsRecord>>(recordList);
        }

        public async Task<IEnumerable<DnsRecord>> ModifyRecordListAsync(string newIp, IEnumerable<DnsRecord> records)
        {
            var rep =  await _client.ModifyRecordBatch(new ModifyRecordBatchRequest()
            {
                RecordIdList = records.Select(m => (ulong?)Convert.ToUInt64(m.Id)).ToArray(),
                Change = "value",
                ChangeTo = newIp
            });
            return records;
        }
    }
}
