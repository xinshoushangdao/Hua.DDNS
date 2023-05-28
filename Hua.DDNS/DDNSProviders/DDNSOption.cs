using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Hua.DDNS.Common.Config.Options;
using Hua.DDNS.DDNSProviders.Namesilo;

namespace Hua.DDNS.DDNSProviders
{
    /// <summary>
    /// domain configuration class
    /// </summary>
    public class DdnsOption
    {
        /// <summary>
        /// platform from  1 Ali 2 Tencent 3
        /// </summary>
        public PlatformEnum Platform { get; set; }

        /// <summary>
        /// domain
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// sub domain, eg. www,git
        /// </summary>
        public string[] SubDomainArray { get; set; }

    }

}
