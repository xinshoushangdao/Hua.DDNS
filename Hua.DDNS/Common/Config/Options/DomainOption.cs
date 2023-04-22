using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hua.DDNS.Common.Config.Options
{
    /// <summary>
    /// domain configuration class
    /// </summary>
    public class DomainOption
    {
        /// <summary>
        /// platform from  1 Ali 2 Tencent
        /// </summary>
        public PlatformEnum Platform { get; set; }

        /// <summary>
        /// Id, the id and key from AliCould or DnsPod
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// domain
        /// </summary>
        public string domain { get; set; }

        /// <summary>
        /// sub domain, eg. www,git
        /// </summary>
        public string[] subDomainArray { get; set; }

    }

}
