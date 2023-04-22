using Hua.DDNS.Common.Config.Options;

namespace Hua.DDNS.Common.Config
{

    /// <summary>
    /// this is a strongly-typed configuration provider 
    /// </summary>
    public class SettingProvider
    {
        private readonly AppOption _app;
        private readonly IConfiguration _configuration;
        public SettingProvider(IConfiguration configuration)
        {
            _configuration = configuration;
            _app = new AppOption();
            _configuration.GetSection("App").Bind(_app);
        }
        
        public AppOption App => _app;
    }

}
