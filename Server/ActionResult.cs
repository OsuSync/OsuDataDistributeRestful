using System.IO;

namespace OsuDataDistributeRestful.Server
{
    internal class ActionResult
    {
        /// <summary>
        /// MIME
        /// </summary>
        public string ContentType { get; set; } = "application/octet-stream";
        public int Code { get; private set; } = 200;

        public Stream Stream { get; private set; }
        public object Data { get; private set; }

        public ActionResult(object a,int code=200)
        {
            ContentType = "text/json";
            Data = a;
            Code = code;
        }

        public ActionResult(Stream s, int code = 200)
        {
            Stream = s;
            Code = code;
        }
    }
}