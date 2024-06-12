namespace Hua.DDNS.DDNSProviders.Dnspod
{
    /// <summary>
    /// domain configuration Dnspod
    /// </summary>
    public class DnspodOption
    {

        /// <summary>
        /// Id, the id and key from AliCould or DnsPod
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Key
        /// </summary>
        public string Key { get; set; }

        /// <summary>
        /// Endpoint dnspod.tencentcloudapi.com
        /// </summary>
        public string Endpoint { get; set; }
    }

}