using Newtonsoft.Json;
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
                    switch (e.ErrorCode)
                    {
                        case 5:
                            Sync.Tools.IO.CurrentIO.WriteColor($"[ODDR]{DefaultLanguage.AllowRequireAdministrator}", ConsoleColor.Red);
                            break;
                        case 32:
                            Sync.Tools.IO.CurrentIO.WriteColor($"[ODDR]{string.Format(DefaultLanguage.PortIsOccupied, Port)}", ConsoleColor.Red);
                            break;
                        default:
                            Sync.Tools.IO.CurrentIO.WriteColor($"[ODDR]{e}", ConsoleColor.Red);
                            break;
                    }
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

#pragma warning disable CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                        Task.Run(() =>
                        {
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
                        });
#pragma warning restore CS4014 // 由于此调用不会等待，因此在调用完成前将继续执行当前方法
                    }
                    catch (HttpListenerException)
                    {
                        continue;
                    }
                }
            }
        }

        protected void ReturnErrorCode(HttpListenerResponse response,ActionResult result)
        {
            response.StatusCode = result.Code;
            using (var sw = new StreamWriter(response.OutputStream))
                sw.Write(JsonConvert.SerializeObject(new {
                    code = result.Code,
                    reason = result.Reason
                }));
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