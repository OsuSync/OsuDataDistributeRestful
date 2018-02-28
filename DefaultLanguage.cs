using Sync.Tools;
using Sync.Tools.ConfigGUI;

namespace OsuDataDistributeRestful
{
    public class DefaultLanguage : I18nProvider
    {
        [ConfigI18n]
        public static LanguageElement AllowLAN = "Allow LAN";

        [ConfigI18n]
        public static LanguageElement EnableFileHttpServer = "Enable file HTTP server";

        [ConfigI18n]
        public static LanguageElement FileServerRootPath = "File server root path";
    }
}