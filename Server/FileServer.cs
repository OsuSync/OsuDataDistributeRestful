using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace OsuDataDistributeRestful.Server
{
    public class FileServer:ServerBase
    {
        public FileServer(int port=10081):base(port)
        {

        }

        protected override void OnResponse(HttpListenerRequest request, HttpListenerResponse response)
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
