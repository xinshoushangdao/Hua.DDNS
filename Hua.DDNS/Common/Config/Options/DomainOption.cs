using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hua.DDNS.Common.Config.Options
{


    public class DomainOption
    {
        /// <summary>
        /// 平台
        /// </summary>
        public string Platform { get; set; }

        /// <summary>
        /// Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// 域名
        /// </summary>
        public string domain { get; set; }

        /// <summary>
        /// 子域列表
        /// </summary>
        public string[] subDomainArray { get; set; }


        /// <summary>
        /// 解析记录类型
        /// </summary>
        public string type { get; set; }

        /// <summary>
        /// 间隔时间 秒
        /// </summary>
        public string time { get; set; }
    }

}
