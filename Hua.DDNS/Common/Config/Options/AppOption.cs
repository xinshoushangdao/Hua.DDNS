using Hua.DDNS.DDNSProviders;

namespace Hua.DDNS.Common.Config.Options
{

    /// <summary>
    /// app configuration class
    /// </summary>
    public class AppOption
    {

        /// <summary>
        /// domain configuration 
        /// </summary>
        public DdnsOption DDNS { get; set; }

    }
}
