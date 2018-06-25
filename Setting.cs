using Sync.Tools;
using Sync.Tools.ConfigurationAttribute;

namespace OsuDataDistributeRestful
{
    public class SettingIni : IConfigurable
    {
        [Bool(RequireRestart = true)]
        public ConfigurationElement AllowLAN
        {
            set => Setting.AllowLAN = bool.Parse(value);
            get => Setting.AllowLAN.ToString();
        }

        [Bool(RequireRestart = true)]
        public ConfigurationElement EnableFileHttpServer
        {
            set => Setting.EnableFileHttpServer = bool.Parse(value);
            get => Setting.EnableFileHttpServer.ToString();
        }

        [Path(IsDirectory = true)]
        public ConfigurationElement FileServerRootPath
        {
            set => Setting.FileServerRootPath = value;
            get => Setting.FileServerRootPath;
        }

        [Integer(MinValue = 5000, MaxValue = 65535, RequireRestart = true)]
        public ConfigurationElement ApiPort
        {
            set => Setting.ApiPort = int.Parse(value);
            get => Setting.ApiPort.ToString();
        }

        [Integer(MinValue = 5000, MaxValue = 65535, RequireRestart = true)]
        public ConfigurationElement FilePort
        {
            set => Setting.FilePort = int.Parse(value);
            get => Setting.FilePort.ToString();
        }

        public void onConfigurationLoad()
        {
        }

        public void onConfigurationReload()
        {
        }

        public void onConfigurationSave()
        {
        }
    }

    internal static class Setting
    {
        public static bool AllowLAN = false;
        public static bool EnableFileHttpServer = false;
        public static string FileServerRootPath = @"..\html";

        public static int ApiPort = 10800;
        public static int FilePort = 10801;
    }
}