using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful.Server
{
    public abstract class ServerBase
    {
        public int Port { get; private set; }
        private bool m_quit = false;

        protected bool AllowLAN { get; set; } = false;

        public ServerBase(int port)
        {
            Port = port;
        }

        public void Start()
        {
            Task.Run(() => Startup());
        }

        public void Stop()
        {
            m_quit = true;
            OnStop();
        }

        private async Task Startup()
        {
            using (HttpListener m_httpd = new HttpListener() { IgnoreWriteExceptions = true })
            {
                if (AllowLAN)
                    m_httpd.Prefixes.Add($"http://+:{Port}/");
                else
                    m_httpd.Prefixes.Add($"http://localhost:{Port}/");

                try
                {
                    m_httpd.Start();
                    OnStarted();
                }
                catch (HttpListenerException e)
                {
                    Sync.Tools.IO.CurrentIO.WriteColor($"[ODDR]{DefaultLanguage.AllowRequireAdministrator}", ConsoleColor.Red);
                    return;
                }

                while (!m_quit)
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
                            OnResponse(request, response);
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

        protected void Return404(HttpListenerResponse response)
        {
            response.StatusCode = 404;
            using (var sw = new StreamWriter(response.OutputStream))
                sw.Write("{\"code\":404}");
        }

        protected void Return500(HttpListenerResponse response)
        {
            response.StatusCode = 500;
            using (var sw = new StreamWriter(response.OutputStream))
                sw.Write("{\"code\":500}");
        }

        protected virtual void OnStarted()
        {
        }

        protected abstract void OnResponse(HttpListenerRequest request, HttpListenerResponse response);

        protected virtual void OnStop()
        {
        }
    }
}