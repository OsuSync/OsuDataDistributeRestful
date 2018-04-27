using System.IO;

namespace OsuDataDistributeRestful.Server
{
    internal class StreamResult
    {
        /// <summary>
        /// MIME
        /// </summary>
        public string ContentType { get; set; } = "application/octet-stream";

        public Stream Data { get; private set; }

        public StreamResult(Stream s)
        {
            Data = s;
        }
    }
}