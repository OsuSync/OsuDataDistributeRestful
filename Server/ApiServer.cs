using Newtonsoft.Json;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful.Server
{
    class ApiServer : ServerBase
    {
        private bool m_http_quit = false;

        private Dictionary<RouteTemplate, Func<ParamCollection, object>> m_route_dict =
            new Dictionary<RouteTemplate, Func<ParamCollection, object>>();

        private int m_port = 10800;

        public ApiServer(int port = 10800) : base(port)
        {
            AllowLAN = Setting.AllowLAN;
            RegisterResource("/api", (p) => m_route_dict.Keys.Select(template => template.Template));
        }

        /// <summary>
        /// Register Resource
        /// </summary>
        /// <param name="route">support template route</param>
        /// <param name="c"></param>
        public void RegisterResource(string route, Func<ParamCollection, object> c)
        {
            m_route_dict.Add(new RouteTemplate(route), c);
        }

        protected override void OnStarted()
        {
            if (AllowLAN)
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

        protected override void OnResponse(HttpListenerRequest request, HttpListenerResponse response)
        {
            var matched_route = m_route_dict.Select(route => new
            {
                Success = route.Key.TryMatch(request.Url.AbsolutePath, out var @params),
                Params = @params,
                Route = route
            }).Where(route_result => route_result.Success).SingleOrDefault();

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
                    response.StatusCode = 200;

                    using (var sw = new StreamWriter(response.OutputStream))
                        sw.Write(json);
                }
            }
            else
            {
                Return404(response);
            }
        }
    }
}
