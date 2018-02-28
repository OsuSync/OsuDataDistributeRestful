using Sync.Tools;
using Sync.Tools.ConfigGUI;
using System;

namespace OsuDataDistributeRestful
{
    public class SettingIni : IConfigurable
    {
        [ConfigBool(NeedRestart = true)]
        public ConfigurationElement AllowLAN { set; get; }
        [ConfigBool(NeedRestart = true)]
        public ConfigurationElement EnableFileHttpServer { get; set; }
        [ConfigPath(IsFilePath = false)]
        public ConfigurationElement FileServerRootPath { get; set; }

        public void onConfigurationLoad()
        {
            try
            {
                Setting.AllowLAN = bool.Parse(AllowLAN);
                Setting.EnableFileHttpServer = bool.Parse(EnableFileHttpServer);
                Setting.FileServerRootPath = FileServerRootPath;
            }
            catch (Exception e)
            {
                onConfigurationSave();
            }
        }

        public void onConfigurationReload()
        {
            onConfigurationLoad();
        }

        public void onConfigurationSave()
        {
            AllowLAN = Setting.AllowLAN.ToString();
            EnableFileHttpServer = Setting.EnableFileHttpServer.ToString();
            FileServerRootPath = Setting.FileServerRootPath;
        }
    }

    internal static class Setting
    {
        public static bool AllowLAN = false;
        public static bool EnableFileHttpServer = false;
        public static string FileServerRootPath = "";
    }
}