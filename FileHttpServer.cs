using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OsuDataDistributeRestful
{
    class FileHttpServer
    {
        
        private bool m_quit = false;

        public FileHttpServer()
        {

        }

        public async void Start()
        {
            using (HttpListener m_httpd = new HttpListener())
            {
                m_httpd.Start();
                m_httpd.Prefixes.Add(@"http://localhost:10801/");
                m_httpd.IgnoreWriteExceptions = true;

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

                        if (request.HttpMethod == "GET")
                        {
                            var filename = Path.Combine(Setting.FileServerRootPath, HttpUtility.UrlDecode(request.RawUrl).Remove(0, 1));
                            if (!filename.Contains("..") && File.Exists(filename))
                            {
                                var ext = Path.GetExtension(filename);
                                response.StatusCode = 200;
                                response.ContentType = GetContentType(ext);

                                using (var fp = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read))
                                {
                                    response.ContentLength64 = fp.Length;
                                    fp.CopyTo(response.OutputStream);
                                }
                            }
                            else
                            {
                                response.StatusCode = 404;
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
                    catch (HttpListenerException)
                    {
                        continue;
                    }
                }
            }
        }

        public void Stop()
        {
            m_quit = true;
        }

        string GetContentType(string fileExtention)
        {
            switch (fileExtention)
            {
                case ".htm":
                case ".html":
                    return "text/html";
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".ogg":
                    return "audio/ogg";
                case ".mp3":
                    return "audio/mpeg";
                default:
                    return "application/octet-stream";
            }
        }
    }
}
