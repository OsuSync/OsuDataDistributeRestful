using Newtonsoft.Json;
using OsuDataDistributeRestful.Server;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful
{
    [SyncSoftRequirePlugin("OsuRTDataProviderPlugin", "RealTimePPDisplayerPlugin", "OsuLiveStatusPanelPlugin")]
    [SyncPluginID("50549ae4-8ba8-4b3b-9d18-9828d43c6523", VERSION)]
    public class OsuDataDistributeRestfulPlugin : Plugin
    {
        public const string PLUGIN_NAME = "OsuDataDistributeRestful";
        public const string PLUGIN_AUTHOR = "KedamavOvO";
        public const string VERSION = "0.0.6";

        private ApiServer apiServer;
        private FileServer fileHttpServer;

        private PluginConfigurationManager m_config_manager;

        public OsuDataDistributeRestfulPlugin() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {
            m_config_manager = new PluginConfigurationManager(this);
            m_config_manager.AddItem(new SettingIni());

            apiServer = new ApiServer();
        }

        #region Initializtion
        private void ORTDP_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "OsuRTDataProvider").FirstOrDefault();
            if (plugin != null)
            {
                new Ortdp.OrtdpResourceInitializer(plugin, apiServer);
            }
            else
                IO.CurrentIO.WriteColor($"[ODDR]Not Found RealTimePPDisplayer", ConsoleColor.Red);
        }

        private void RTPPD_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "RealTimePPDisplayer").FirstOrDefault();
            if (plugin != null)
            {
                new Rtppd.RtppdResourceInitializer(plugin, apiServer);
            }
            else
                IO.CurrentIO.WriteColor($"[ODDR]Not Found RealTimePPDisplayer", ConsoleColor.Red);
        }

        private void OLSP_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name== "OsuLiveStatusPanelPlugin").FirstOrDefault();
            if (plugin != null)
            {
                new Olsp.OlspResourceInitializer(plugin, apiServer);
            }
            else
                IO.CurrentIO.WriteColor($"[ODDR]Not Found OsuLiveStatusPanel", ConsoleColor.Red);
        }
        #endregion

        private void Initialize()
        {
            ORTDP_Initialize();
            RTPPD_Initialize();
            OLSP_Initialize();

            if (Setting.EnableFileHttpServer)
            {
                fileHttpServer = new FileServer();
                Task.Run(()=>fileHttpServer.Start());
            }
        }

        public override void OnEnable()
        {
            Sync.Tools.IO.CurrentIO.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
            Initialize();


            base.EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(e=> apiServer.Start());
        }

        public override void OnExit()
        {
            if (Setting.EnableFileHttpServer)
                fileHttpServer.Stop();
        }
    }
}
