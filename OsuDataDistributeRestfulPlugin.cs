using OsuDataDistributeRestful.Api;
using OsuDataDistributeRestful.Server;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;

namespace OsuDataDistributeRestful
{
    [SyncSoftRequirePlugin("OsuRTDataProviderPlugin", "RealTimePPDisplayerPlugin", "OsuLiveStatusPanelPlugin")]
    [SyncPluginID("50549ae4-8ba8-4b3b-9d18-9828d43c6523", VERSION)]
    public class OsuDataDistributeRestfulPlugin : Plugin
    {
        public const string PLUGIN_NAME = "OsuDataDistributeRestful";
        public const string PLUGIN_AUTHOR = "KedamavOvO";
        public const string VERSION = "0.4.0";

        public static readonly Version MIN_ORTDP_VERSION = Version.Parse("1.4.3");
        public static readonly Version MIN_RTPPD_VERSION = Version.Parse("1.7.0");

        public ApiServer ApiServer { get; private set; }
        private FileServer fileHttpServer;

        private PluginConfigurationManager m_config_manager;

        public OsuDataDistributeRestfulPlugin() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {
            I18n.Instance.ApplyLanguage(new DefaultLanguage());
            m_config_manager = new PluginConfigurationManager(this);
            m_config_manager.AddItem(new SettingIni());

            ApiServer = new ApiServer(Setting.ApiPort);
            EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(e => ApiServer.Start());
            if (Setting.AllowLAN)
                EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(e => PrintLanAddress());

            EventBus.BindEvent<PluginEvents.LoadCompleteEvent>(e => Initialize());

            if (Setting.EnableFileHttpServer)
            {
                fileHttpServer = new FileServer(Setting.FilePort);
                fileHttpServer.Start();
            }
        }

        private void PrintLanAddress()
        {
            //Display IP Address
            var ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork).Distinct();
            int n = 1;
            foreach (var ip in ips)
            {
                bool recommend = ip.ToString().StartsWith("192.168.");
                IO.CurrentIO.WriteColor($"[ODDR]IP {n++}:{ip}", recommend ? ConsoleColor.Green : ConsoleColor.White);
            }
        }

        #region Initializtion

        private Plugin ORTDP_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "OsuRTDataProvider").FirstOrDefault();
            if (plugin != null)
            {
                if (plugin.GetType().Assembly.GetName().Version > MIN_ORTDP_VERSION)
                {
                    ApiServer.RegisterResource(new OrtdpApis(plugin));
                    return plugin;
                }
                else
                {
                    IO.DefaultIO.WriteColor(string.Format(DefaultLanguage.MINIMUM_VERSION_HINT, "OsuRTDataProvider", MIN_ORTDP_VERSION), ConsoleColor.Yellow);
                }
            }
            else
            {
                IO.CurrentIO.WriteColor($"[ODDR]Not Found OsuRTDataProvider", ConsoleColor.Red);
            }

            return null;
        }

        private Plugin RTPPD_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "RealTimePPDisplayer").FirstOrDefault();
            if (plugin != null)
            {
                if (plugin.GetType().Assembly.GetName().Version > MIN_RTPPD_VERSION)
                {
                    ApiServer.RegisterResource(new RtppdApis(plugin));
                    return plugin;
                }
                else
                {
                    IO.DefaultIO.WriteColor(string.Format(DefaultLanguage.MINIMUM_VERSION_HINT, "RealTimePPDisplayer", MIN_RTPPD_VERSION), ConsoleColor.Yellow);
                }
            }
            else
            {
                IO.CurrentIO.WriteColor($"[ODDR]Not Found RealTimePPDisplayer", ConsoleColor.Red);
            }

            return null;
        }

        private Plugin OLSP_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "OsuLiveStatusPanelPlugin").FirstOrDefault();
            if (plugin != null)
            {
                ApiServer.RegisterResource(new OlspApis(plugin));
                return plugin;
            }

            IO.CurrentIO.WriteColor($"[ODDR]Not Found OsuLiveStatusPanel", ConsoleColor.Red);
            return null;
        }

        #endregion Initializtion

        private void Initialize()
        {
            var ortdp = ORTDP_Initialize();
            var rtppd = RTPPD_Initialize();
            var olsp = OLSP_Initialize();

            if(ortdp !=null && rtppd != null)
            {
                ApiServer.RegisterResource(new ExtraApis(ortdp,rtppd));
            }
        }

        public override void OnEnable()
        {
            Sync.Tools.IO.CurrentIO.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
        }

        public override void OnExit()
        {
            if (Setting.EnableFileHttpServer)
                fileHttpServer?.Stop();
        }
    }
}