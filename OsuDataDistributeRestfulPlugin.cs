using Newtonsoft.Json;
using RealTimePPDisplayer;
using Sync.Command;
using Sync.Plugins;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful
{
    [SyncPluginDependency("8eb9e8e0-7bca-4a96-93f7-6408e76898a9", Version = "^1.2.3", Require = true)]
    [SyncPluginID("4b045b1c-7ab2-41a7-9f80-7e79c0d7768a", VERSION)]
    public class OsuDataDistributeRestfulPlugin : Plugin
    {
        public const string PLUGIN_NAME = "OsuDataDistributeRestful";
        public const string PLUGIN_AUTHOR = "KedamavOvO";
        public const string VERSION = "0.0.2";

        private HttpListener m_httpd=new HttpListener();
        private Dictionary<string, Func<object>> m_url_dict = new Dictionary<string, Func<object>>();

        private RestfulDisplayer m_displayer;
        private PluginConfigurationManager m_config_manager;

        public OsuDataDistributeRestfulPlugin() : base(PLUGIN_NAME, PLUGIN_AUTHOR)
        {

        }

        private void Initialize()
        {
            m_config_manager = new PluginConfigurationManager(this);
            m_config_manager.AddItem(new SettingIni());

            var plugin=getHoster().EnumPluings().Where(p => p.Name == "RealTimePPDisplayer").FirstOrDefault();
            if (plugin is RealTimePPDisplayerPlugin rtppd)
            {
                rtppd.RegisterDisplayer("restful", (id) => m_displayer = new RestfulDisplayer());
                RegisterResource("/api/is_play", () => new { IsPlay=m_displayer.IsPlay });
                RegisterResource("/api/pp", () => m_displayer.PPTuple);
                RegisterResource("/api/hit_count", () => m_displayer.HitCountTuple);
                RegisterResource("/api/pp_formated",()=>new {Formated=m_displayer.StringPP});
                RegisterResource("/api/hit_count_formated", () => new { Formated = m_displayer.StringHitCount });
            }
            else
                IO.CurrentIO.WriteColor($"Not Found RealTimePPDisplayer", ConsoleColor.Red);
        }

        private void RegisterResource(string uri,Func<object> c)
        {
            m_url_dict.Add(uri, c);
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
            while (true)
            {
                var ctx = await m_httpd.GetContextAsync();
                var request = ctx.Request;
                var response = ctx.Response;
                response.AppendHeader("Access-Control-Allow-Origin","*");
                response.AppendHeader("Access-Control-Allow-Methods", "GET");
                response.AppendHeader("Access-Control-Allow-Headers", "x-requested-with,content-type");

                response.ContentEncoding = Encoding.UTF8;
                response.ContentType = "text/json; charset=UTF-8";

                if(request.HttpMethod=="GET")
                {
                    if (m_url_dict.TryGetValue(request.Url.AbsolutePath, out var func))
                    {
                        var json=JsonConvert.SerializeObject(func());
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
    }
}
