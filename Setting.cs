using Sync.Tools;
using System;

namespace OsuDataDistributeRestful
{
    public class SettingIni : IConfigurable
    {
        public ConfigurationElement AllowLAN { set; get; }
        public ConfigurationElement EnableSongsHttpServer { get; set; }
        public ConfigurationElement OsuSongsPath { get; set; }

        public void onConfigurationLoad()
        {
            try
            {
                Setting.AllowLAN = bool.Parse(AllowLAN);
                Setting.EnableSongsHttpServer = bool.Parse(EnableSongsHttpServer);
                Setting.OsuSongsPath = OsuSongsPath;
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
            EnableSongsHttpServer = Setting.EnableSongsHttpServer.ToString();
            OsuSongsPath = Setting.OsuSongsPath;
        }
    }

    internal static class Setting
    {
        public static bool AllowLAN = false;
        public static bool EnableSongsHttpServer = false;
        public static string OsuSongsPath = "";
    }
}