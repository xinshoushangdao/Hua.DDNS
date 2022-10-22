using Hua.DDNS.Common.Config.Options;

namespace Hua.DDNS.Common.Config
{
    public class SettingProvider
    {
        private readonly AppOption _app;
        public SettingProvider(IConfiguration configuration)
        {
            _app = new AppOption();
            configuration.GetSection("App").Bind(_app);
        }
        
        public AppOption App => _app;
    }

}
