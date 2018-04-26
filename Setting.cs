using Sync.Tools;
using Sync.Tools.ConfigGUI;
using System;

namespace OsuDataDistributeRestful
{
    public class SettingIni : IConfigurable
    {
        [Bool(RequireRestart = true)]
        public ConfigurationElement AllowLAN
        {
            set => Setting.AllowLAN = bool.Parse(value);
            get => Setting.AllowLAN.ToString(); }

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
        public static string FileServerRootPath = "";
    }
}