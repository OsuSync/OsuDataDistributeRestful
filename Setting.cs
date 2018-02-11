using Sync.Tools;
using System;

namespace OsuDataDistributeRestful
{
    public class SettingIni : IConfigurable
    {
        public ConfigurationElement AllowLAN { set; get; }

        public void onConfigurationLoad()
        {
            try
            {
                Setting.AllowLAN = bool.Parse(AllowLAN);
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
        }
    }

    internal static class Setting
    {
        public static bool AllowLAN = false;
    }
}