using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AlibabaCloud.OpenApiClient.Models;
using AlibabaCloud.SDK.Alidns20150109;
using AlibabaCloud.SDK.Alidns20150109.Models;
using AutoMapper;
using Hua.DDNS.Common.Config.Options;
using Hua.DDNS.Models;
using Microsoft.Extensions.Options;

namespace Hua.DDNS.DDNSProviders.Ali
{

    /// <summary>
    /// DDNSProvider for Ali
    /// </summary>
    public class AliDdnsProvider : IDdnsProvider
    {
        private readonly Client _client;
        private readonly AliDdnsOption _aliDDNSOption;
        private readonly DdnsOption _ddnsOption;
        private readonly IMapper _mapper;

        public AliDdnsProvider(IOptions<AliDdnsOption> aliDDNSOption, IMapper mapper,IOptions<DdnsOption> ddnsOption)
        {
            _aliDDNSOption = aliDDNSOption.Value;
            _ddnsOption = ddnsOption.Value;
            _mapper = mapper;


            _client = new Client(new Config()
            {
                // 您的 AccessKey ID
                AccessKeyId = _aliDDNSOption.Id,
                // 您的 AccessKey Secret
                AccessKeySecret = _aliDDNSOption.Key,
                Endpoint = _aliDDNSOption.Endpoint,//alidns.cn-beijing.aliyuncs.com
            });
        }

        /// <summary>
        /// 获取解析记录列表
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<DnsRecord>?> GetRecordListAsync()
        {
            var record =  (await _client.DescribeDomainRecordsAsync(new DescribeDomainRecordsRequest()
            {
                DomainName = _ddnsOption.Domain
            })).Body.DomainRecords.Record;

            return _mapper.Map<IEnumerable<DnsRecord>>(record);
        }

        /// <summary>
        /// 变更解析记录列表
        /// </summary>
        /// <param name="newIp"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        public async Task<IEnumerable<DnsRecord>> ModifyRecordListAsync(string newIp, IEnumerable<DnsRecord> records)
        {
            foreach (var aliDomainRecord in records)
            {
                await _client.UpdateDomainRecordAsync(_mapper.Map<UpdateDomainRecordRequest>(aliDomainRecord));
            }

            return records;
        }
    }
}
