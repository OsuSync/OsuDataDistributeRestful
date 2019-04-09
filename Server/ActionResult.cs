using Newtonsoft.Json;
using System.IO;

namespace OsuDataDistributeRestful.Server
{
    public class ActionResult
    {
        /// <summary>
        /// MIME
        /// </summary>
        public string ContentType { get; set; } = "application/octet-stream";

        public int Code { get; private set; } = 200;

        public string Reason { get; private set; } = string.Empty;

        public object Data { get; private set; }

        public Formatting Formatting { get; set; } = Formatting.None;

        public ActionResult(object a)
        {
            ContentType = "text/json; charset=UTF-8";
            Data = a;
            Code = 200;
        }

        public ActionResult(object a, int code, string reason)
        {
            ContentType = "text/json; charset=UTF-8";
            Data = a;
            Code = code;
            Reason = reason;
        }

        public ActionResult(Stream s, int code = 200)
        {
            Data = s;
            Code = code;
        }
    }
}