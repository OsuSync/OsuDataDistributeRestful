using Newtonsoft.Json;
using OsuDataDistributeRestful.Server.Api;
using Sync.Tools;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace OsuDataDistributeRestful.Server
{
    internal class ApiServer : ServerBase
    {
        private Dictionary<RouteTemplate, Method> m_route_dict =
            new Dictionary<RouteTemplate, Method>();

        struct Method
        {
            public MethodInfo MethodInfo;
            public object Instance;
        }
        class ParamCollection : Dictionary<string, string>
        {
            public ParamCollection()
            {
            }

            public ParamCollection(Dictionary<string, string> dict) : base(dict)
            {
            }

            public string TryGetValue(string key)
            {
                if (TryGetValue(key, out string val))
                    return val;
                return null;
            }
        }
        class RouteTemplate
        {
            private struct TemplateNode
            {
                public string Key;
                public bool IsTemplate;
            };

            private List<TemplateNode> template_breaked = new List<TemplateNode>();
            public string Template { get; set; }

            public RouteTemplate(string template)
            {
                Template = template;
                template_breaked = template.Split('/')
                    .Where(n => !string.IsNullOrWhiteSpace(n))
                    .Select(n => new TemplateNode()
                    {
                        Key = n.Replace("{", "").Replace("}", ""),
                        IsTemplate = n[0] == '{' && n[n.Length - 1] == '}'
                    }).ToList();
            }

            public bool TryMatch(string path, out ParamCollection @params)
            {
                @params = new ParamCollection();
                var breaked = path.Split('/').Where(n => !string.IsNullOrWhiteSpace(n));
                int i = 0;

                if (breaked.Count() != template_breaked.Count)
                {
                    @params = null;
                    return false;
                }

                foreach (var b in breaked)
                {
                    TemplateNode template = template_breaked[i];

                    if (template.IsTemplate)
                    {
                        string key = template.Key;
                        string value = b;
                        @params.Add(key, b);
                    }
                    else
                    {
                        if (template.Key != b)
                        {
                            @params = null;
                            return false;
                        }
                    }
                    i++;
                }
                return true;
            }
        }

        class Apis : IApi
        {
            private readonly ApiServer m_apiServer;

            public Apis(ApiServer apiServer)
            {
                m_apiServer = apiServer;
            }

            [Route("/api")]
            public object OutputAllApi()
            {
                return m_apiServer.m_route_dict.Keys.Select(template => template.Template);
            }
        }

        public ApiServer(int port = 10800) : base(port)
        {
            AllowLAN = Setting.AllowLAN;
            RegisterResource(new Apis(this));
        }

        /// <summary>
        /// Register Resource
        /// </summary>
        /// <param name="route">support template route</param>
        /// <param name="c"></param>
        public void RegisterResource(IApi api)
        {
            Type api_type = api.GetType();
            RouteAttribute api_route = api_type.GetCustomAttribute<RouteAttribute>()??new RouteAttribute("/");

            foreach(var method in api_type.GetMethods())
            {
                RouteAttribute method_route = method.GetCustomAttribute<RouteAttribute>();
                if (method_route != null)
                {
                    string route = method_route.Route;
                    if (route[0] == '/')
                        route=route.Remove(0,1);

                    route = Path.Combine(api_route.Route, route).Replace(Path.DirectorySeparatorChar,'/');

                    RouteTemplate route_template = new RouteTemplate(route);
                    m_route_dict.Add(route_template, new Method() { Instance = api, MethodInfo = method });
                }
            }
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
            }).Where(route_result => route_result.Success).FirstOrDefault();

            if (matched_route != null)
            {
                try
                {
                    var result = CallMethod(matched_route.Route.Value, matched_route.Params);
                    response.ContentType = result.ContentType;

                    if (result.Stream != null)
                    {
                        result.Stream.CopyTo(response.OutputStream);
                        result.Stream.Dispose();
                    }
                    else if (result.Data != null)
                    {
                        var json = JsonConvert.SerializeObject(result.Data);
                        response.StatusCode = result.Code;

                        using (var sw = new StreamWriter(response.OutputStream))
                            sw.Write(json);
                    }
                    else
                    {
                        Return404(response);
                    }
                }
                catch(Exception e)
                {
                    IO.CurrentIO.WriteColor(e.ToString(), ConsoleColor.Yellow);
                    Return500(response);
                }
            }
            else
            {
                Return404(response);
            }
        }

        private ActionResult CallMethod(Method method,ParamCollection @params)
        {
            var params_instance=method.MethodInfo.GetParameters()
                .Select(p => TypeConvert(p.ParameterType, @params.TryGetValue(p.Name)))
                .ToArray();
            var ret = method.MethodInfo.Invoke(method.Instance, params_instance);

            if (!(ret is ActionResult))
                ret = new ActionResult(ret);
            return ret as ActionResult;
        }

        private object TypeConvert(Type type,string str)
        {
            object val=str;

            try
            {
                if (type == typeof(int))
                    val = int.Parse(str);
                else if (type == typeof(short))
                    val = short.Parse(str);
                else if (type == typeof(double))
                    val = double.Parse(str, CultureInfo.InvariantCulture);
                else if (type == typeof(float))
                    val = float.Parse(str, CultureInfo.InvariantCulture);
            }
            catch(FormatException e)
            {
                IO.CurrentIO.WriteColor($"[ODDR]{e}", ConsoleColor.Yellow);
            }

            return val;
        }
    }
}