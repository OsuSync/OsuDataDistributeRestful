using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuDataDistributeRestful
{
    public class DefaultLanguage : I18nProvider
    {
        public static LanguageElement AllowRequireAdministrator = "AllowLAN requires that the administrator run sync";
        public static LanguageElement PortIsOccupied = "Port {0} is occupied";
        public static LanguageElement MINIMUM_VERSION_HINT = "[ODDR]The version of {0} is required to be after {1}.";

        public static GuiLanguageElement AllowLAN = "Allow LAN";
        public static GuiLanguageElement EnableFileHttpServer = "Enable file HTTP server";
        public static GuiLanguageElement FileServerRootPath = "File server root path";
        public static GuiLanguageElement ApiPort = "Api Server Port";
        public static GuiLanguageElement FilePort = "File Server Port";
    }
}