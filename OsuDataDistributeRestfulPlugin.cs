using Newtonsoft.Json;
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
    [SyncPluginDependency("8eb9e8e0-7bca-4a96-93f7-6408e76898a9", Version = "^1.3.2", Require = true)]//RTPPD
    [SyncPluginID("4b045b1c-7ab2-41a7-9f80-7e79c0d7768a", VERSION)]
    public class OsuDataDistributeRestfulPlugin : Plugin
    {
        public const string PLUGIN_NAME = "OsuDataDistributeRestful";
        public const string PLUGIN_AUTHOR = "KedamavOvO";
        public const string VERSION = "0.0.4";

        private bool m_http_quit = false;
        private HttpListener m_httpd=new HttpListener();
        private Dictionary<string, Func<ParamCollection, object>> m_url_dict = new Dictionary<string, Func<ParamCollection,object>>();

        private SongsHttpServer songsHttpServer;

        private PluginConfigurationManager m_config_manager;

        public OsuDataDistributeRestfulPlugin() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {
        }

        #region Initializtion
        private void ORTDP_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "OsuRTDataProvider").FirstOrDefault();
            if (plugin != null)
            {
                new Ortdp.OrtdpResourceInitializer(plugin, this);
            }
            else
                IO.CurrentIO.WriteColor($"[ODDR]Not Found RealTimePPDisplayer", ConsoleColor.Red);
        }

        private void RTPPD_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name == "RealTimePPDisplayer").FirstOrDefault();
            if (plugin != null)
            {
                new Rtppd.RtppdResourceInitializer(plugin, this);
            }
            else
                IO.CurrentIO.WriteColor($"[ODDR]Not Found RealTimePPDisplayer", ConsoleColor.Red);
        }

        private void OLSP_Initialize()
        {
            var plugin = getHoster().EnumPluings().Where(p => p.Name== "OsuLiveStatusPanelPlugin").FirstOrDefault();
            if (plugin != null)
            {
                new Olsp.OlspResourceInitializer(plugin, this);
            }
            else
                IO.CurrentIO.WriteColor($"[ODDR]Not Found OsuLiveStatusPanel", ConsoleColor.Red);
        }
        #endregion

        private void Initialize()
        {
            m_config_manager = new PluginConfigurationManager(this);
            m_config_manager.AddItem(new SettingIni());

            ORTDP_Initialize();
            RTPPD_Initialize();
            OLSP_Initialize();

            RegisterResource("/api",(p)=>m_url_dict.Keys);

            if (Setting.EnableSongsHttpServer)
            {
                songsHttpServer = new SongsHttpServer();
                songsHttpServer.Start();
            }
        }

        public void RegisterResource(string uri,Func<ParamCollection,object> c)
        {
            m_url_dict.Add(uri, c);
        }

        public void RemappingResource(string uri,string target_uri)
        {
            m_url_dict[target_uri] = m_url_dict[uri];
        }

        public override async void OnEnable()
        {
            Sync.Tools.IO.CurrentIO.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);

            Initialize();
            if(Setting.AllowLAN)
                m_httpd.Prefixes.Add(@"http://+:10800/");
            else
                m_httpd.Prefixes.Add(@"http://localhost:10800/");

            m_httpd.Start();
            while (!m_http_quit)
            {
                var ctx = await m_httpd.GetContextAsync();
                var request = ctx.Request;
                var response = ctx.Response;
                response.AppendHeader("Access-Control-Allow-Origin", "*");
                response.AppendHeader("Access-Control-Allow-Methods", "GET");
                response.AppendHeader("Access-Control-Allow-Headers", "x-requested-with,content-type");

                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "text/json; charset=UTF-8";

                if(request.HttpMethod=="GET")
                {
                    if (m_url_dict.TryGetValue(request.Url.AbsolutePath, out var func))
                    {
                        var @params = ParseUriParams(request.Url);
                        var json=JsonConvert.SerializeObject(func(@params));
                        ctx.Response.StatusCode = 200;

                        using (var sw = new StreamWriter(response.OutputStream))
                            sw.Write(json);
                    }
                    else
                    {
                        ctx.Response.StatusCode = 404;
                        using (var sw = new StreamWriter(response.OutputStream))
                            sw.Write("{\"code\":404}");
                    }
                }
                else
                {
                    response.StatusCode = 403;
                    using (var sw = new StreamWriter(response.OutputStream))
                        sw.Write("{\"code\":403}");
                }
                response.OutputStream.Close();
            }
        }

        private ParamCollection ParseUriParams(Uri uri)
        {
            ParamCollection dictionary = new ParamCollection();
            var params_str = uri.Query;

            if (!string.IsNullOrWhiteSpace(params_str))
            {
                string[] @params = params_str.Remove(0, 1).Split('&');

                foreach (var param in @params)
                {
                    var t = param.Split('=');
                    try
                    {
                        dictionary[t[0]] = t[1];
                    }
                    catch (IndexOutOfRangeException e)
                    {
                        dictionary[t[0]] = null;
                    }
                }
            }
            return dictionary;
        }

        public override void OnExit()
        {
            if (Setting.EnableSongsHttpServer)
                songsHttpServer.Stop();
            m_http_quit = true;
            m_httpd.Stop();
        }
    }
}
