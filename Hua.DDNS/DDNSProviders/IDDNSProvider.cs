using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hua.DDNS.Models;

namespace Hua.DDNS.DDNSProviders
{

    /// <summary>
    /// Dynamic domain name resolution provider
    /// </summary>
    public interface IDdnsProvider
    {

        /// <summary>
        /// 获取域名解析记录列表
        /// </summary>
        /// <returns></returns>

        Task<IEnumerable<DnsRecord>?> GetRecordListAsync();

        /// <summary>
        /// 修改域名解析记录
        /// </summary>
        /// <param name="newIp"></param>
        /// <param name="records"></param>
        /// <returns></returns>
        Task<IEnumerable<DnsRecord>> ModifyRecordListAsync(string newIp, IEnumerable<DnsRecord> records);

    }
}
