using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful
{
    class StreamResult
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
