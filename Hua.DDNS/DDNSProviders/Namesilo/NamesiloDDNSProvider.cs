using System.Xml;
using AutoMapper;
using Hua.DDNS.Models;
using Microsoft.Extensions.Options;

namespace Hua.DDNS.DDNSProviders.Namesilo
{
    /// <summary>
    /// DDNSProvider for namesilo
    /// </summary>
    public class NamesiloDdnsProvider : IDdnsProvider
    {
        public readonly NamesiloOption _namesiloOption;
        public readonly DdnsOption _ddnsOption;

        public NamesiloDdnsProvider(IOptions<NamesiloOption> namesiloOption, IOptions<DdnsOption> ddnsOption)
        {
            _ddnsOption = ddnsOption.Value;
            _namesiloOption = namesiloOption.Value;
        }


        public async Task<IEnumerable<DnsRecord>?> GetRecordListAsync()
        {
            var client = new HttpClient();
            var response =
                await client.GetAsync($"https://www.namesilo.com/api/dnsListRecords?version=1&type=xml&key={_namesiloOption.ApiKey}&domain={_ddnsOption.Domain}");
            var content = response.Content.ReadAsStringAsync().Result;

            var reply = new XmlDocument();
            reply.LoadXml(content);
            var status = reply.SelectSingleNode("/namesilo/reply/code/text()");
            if (status == null)
            {
                return null;
            }

            if (status.Value != "300")
            {
                throw new Exception($"Failed to retrieve value. Check API key.{status}");
            }

            var records = reply.SelectNodes($"/namesilo/reply/resource_record/host");
            if (records == null)
            {
                return new List<DnsRecord>();
            }

            return (from record in records.Cast<XmlNode>()
                let subDomain = record.ParentNode.SelectSingleNode("host/text()").Value.Replace(_ddnsOption.Domain, "")
                    where _ddnsOption.SubDomainArray.Contains(subDomain)
                select new DnsRecord
                {
                    Id = record.ParentNode.SelectSingleNode("record_id/text()").Value,
                    Ip = record.ParentNode.SelectSingleNode("value/text()").Value,
                    Host = record.ParentNode.SelectSingleNode("host/text()").Value,
                    Domain = _ddnsOption.Domain,
                    TTL = record.ParentNode.SelectSingleNode("ttl/text()").Value,
                    SubDomain = subDomain,
                }).ToList();
        }

        public async Task<IEnumerable<DnsRecord>> ModifyRecordListAsync(string newIp, IEnumerable<DnsRecord> records)
        {
            foreach (var dnsRecord in records)
            {
                using var client = new HttpClient();
                {
                    var host = dnsRecord.Host[..(dnsRecord.Host.Length - dnsRecord.Domain.Length - 1)];
                    var request =
                        $"https://www.namesilo.com/api/dnsUpdateRecord?version=1&type=xml&key={_namesiloOption.ApiKey}&domain={dnsRecord.Domain}&rrid={dnsRecord.Id}&rrhost={host}&rrvalue={newIp}&rrttl={dnsRecord.TTL}";
                    //Console.WriteLine(request);
                    var response = await client.GetAsync(request);
                    var content = await response.Content.ReadAsStringAsync();

                    var reply = new XmlDocument();
                    reply.LoadXml(content);
                    var status = reply.SelectSingleNode("/namesilo/reply/code/text()");
                    if (status == null)
                    {
                        await Console.Error.WriteLineAsync($"Failed to update record: '{dnsRecord.Id}' with Ip: '{newIp}'.");
                        continue; //return false;
                    }

                    if (status.Value == "300") continue;
                }

                await Console.Error.WriteLineAsync($"Failed to update record: '{dnsRecord.Id}' with Ip: '{newIp}'.");
                continue; //return false;
            }

            return records;
        }
    }
}