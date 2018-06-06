using Sync.Tools;
using Sync.Tools.ConfigGUI;

namespace OsuDataDistributeRestful
{
    public class DefaultLanguage : I18nProvider
    {
        public static LanguageElement AllowRequireAdministrator = "AllowLAN requires that the administrator run sync";
        public static LanguageElement PortIsOccupied = "Port {0} is occupied";

        public static GuiLanguageElement AllowLAN = "Allow LAN";
        public static GuiLanguageElement EnableFileHttpServer = "Enable file HTTP server";
        public static GuiLanguageElement FileServerRootPath = "File server root path";
    }
}