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
    [SyncSoftRequirePlugin("OsuRTDataProviderPlugin", "RealTimePPDisplayerPlugin", "OsuLiveStatusPanelPlugin")]
    [SyncPluginID("50549ae4-8ba8-4b3b-9d18-9828d43c6523", VERSION)]
    public class OsuDataDistributeRestfulPlugin : Plugin
    {
        public const string PLUGIN_NAME = "OsuDataDistributeRestful";
        public const string PLUGIN_AUTHOR = "KedamavOvO";
        public const string VERSION = "0.0.6";

        private bool m_http_quit = false;

        private Dictionary<RouteTemplate, Func<ParamCollection, object>> m_route_dict = 
            new Dictionary<RouteTemplate, Func<ParamCollection, object>>();

        private FileHttpServer fileHttpServer;

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

            RegisterResource("/api",(p)=>m_route_dict.Keys.Select(template=>template.Template));

            RegisterResource("/test/api/rtppd/{id}/pp", (p) => p);

            if (Setting.EnableFileHttpServer)
            {
                fileHttpServer = new FileHttpServer();
                Task.Run(()=>fileHttpServer.Start());
            }
        }

        /// <summary>
        /// Register Resource
        /// </summary>
        /// <param name="route">support template route</param>
        /// <param name="c"></param>
        public void RegisterResource(string route,Func<ParamCollection, object> c)
        {
            m_route_dict.Add(new RouteTemplate(route), c);
        }

        private async void StartApiHttpServer()
        {
            using (HttpListener m_httpd = new HttpListener() { IgnoreWriteExceptions = true })
            {
                if (Setting.AllowLAN)
                    m_httpd.Prefixes.Add(@"http://+:10800/");
                else
                    m_httpd.Prefixes.Add(@"http://localhost:10800/");

                try
                {
                    m_httpd.Start();
                    if (Setting.AllowLAN)
                    {
                        //Display IP Address
                        var ips = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(ip => ip.AddressFamily == AddressFamily.InterNetwork);
                        int n = 1;
                        foreach (var ip in ips)
                        {
                            IO.CurrentIO.Write($"[ODDR]IP {n++}:{ip}");
                        }
                    }
                }
                catch (HttpListenerException e)
                {
                    Sync.Tools.IO.CurrentIO.WriteColor($"[ODDR]{DefaultLanguage.AllowRequireAdministrator}", ConsoleColor.Red);
                    return;
                }


                while (!m_http_quit)
                {
                    try
                    {
                        var ctx = await m_httpd.GetContextAsync();
                        var request = ctx.Request;
                        var response = ctx.Response;
                        response.AppendHeader("Access-Control-Allow-Origin", "*");
                        response.AppendHeader("Access-Control-Allow-Methods", "GET");
                        response.AppendHeader("Access-Control-Allow-Headers", "x-requested-with,content-type");

                        response.ContentEncoding = Encoding.UTF8;
                        response.ContentType = "text/json; charset=UTF-8";

                        if (request.HttpMethod == "GET")
                        {
                            var matched_route = m_route_dict.Select(route => new
                            {
                                Success = route.Key.TryMatch(request.Url.AbsolutePath, out var @params),
                                Params = @params,
                                Route =route
                            })
                            .Where(route_result=>route_result.Success).SingleOrDefault();

                            if (matched_route != null)
                            {
                                var result = matched_route.Route.Value(matched_route.Params);
                                if (result is StreamResult sr)
                                {
                                    if (sr.Data != null)
                                    {
                                        response.ContentType = sr.ContentType;
                                        sr.Data.CopyTo(response.OutputStream);
                                        sr.Data.Dispose();
                                    }
                                    else
                                    {
                                        Return404(response);
                                    }
                                }
                                else
                                {
                                    var json = JsonConvert.SerializeObject(result);
                                    ctx.Response.StatusCode = 200;

                                    using (var sw = new StreamWriter(response.OutputStream))
                                        sw.Write(json);
                                }
                            }
                            else
                            {
                                Return404(response);
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
                    catch (HttpListenerException)
                    {
                        continue;
                    }
                }
            }
        }

        public override void OnEnable()
        {
            Sync.Tools.IO.CurrentIO.WriteColor(PLUGIN_NAME + " By " + PLUGIN_AUTHOR, ConsoleColor.DarkCyan);
            Initialize();


            base.EventBus.BindEvent<PluginEvents.ProgramReadyEvent>(e=> Task.Run(() => StartApiHttpServer()));
        }

        public void Return404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            using (var sw = new StreamWriter(response.OutputStream))
                sw.Write("{\"code\":404}");
        }

        public override void OnExit()
        {
            m_http_quit = true;
            if (Setting.EnableFileHttpServer)
                fileHttpServer.Stop();
        }
    }
}
