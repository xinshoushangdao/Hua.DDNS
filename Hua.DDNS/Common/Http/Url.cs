using Hua.DDNS.Common.Config;
using Hua.DDNS.Start;

namespace Hua.DDNS.Common.Http
{
    public class Url
    {
        private readonly SettingProvider _settingProvider;

        public Url(SettingProvider settingProvider)
        {
            _settingProvider = settingProvider;
        }

    }
}
