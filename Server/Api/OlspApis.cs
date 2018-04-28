using OsuDataDistributeRestful.Server;
using OsuDataDistributeRestful.Server.Api;
using OsuLiveStatusPanel;
using Sync.Plugins;
using System.IO;
using System.Linq;

namespace OsuDataDistributeRestful.Api
{
    [Route("/api/olsp")]
    internal class OlspApis : IApi
    {
        private OsuLiveStatusPanelPlugin olsp;

        public OlspApis(Plugin olsp_plguin)
        {
            olsp = olsp_plguin as OsuLiveStatusPanelPlugin;
        }

        [Route("/{providable_data_name}")]
        public ActionResult GetDictValue(string providable_data_name)
        {
            if (!olsp.EnumProvidableDataName().Any(p => p == providable_data_name))
                return new ActionResult(new { code = 404}, 404);

            var result = olsp.GetData(providable_data_name);
            return new ActionResult(new
            {
                status = result != null,
                value = result
            });
        }

        [Route("/image/background")]
        public ActionResult GetBackgoundImage()
        {
            var result = olsp.GetData("olsp_bg_path") as string;
            if (string.IsNullOrEmpty(result)) return new ActionResult(null);

            if (File.Exists(result))
            {
                var fs = File.Open(result, FileMode.Open, FileAccess.Read, FileShare.Read);
                string ext = Path.GetExtension(result);

                return new ActionResult(fs)
                {
                    ContentType = GetContentType(ext)
                };
            }
            return new ActionResult(new { code = 404, message = "background image not found" }, 404);
        }

        [Route("/image/output")]
        public ActionResult GetOuputBackgoundImage()
        {
            var result = olsp.GetData("olsp_bg_save_path") as string;
            if (string.IsNullOrEmpty(result)) return new ActionResult(null);

            if (File.Exists(result))
            {
                var fs = File.Open(result, FileMode.Open, FileAccess.Read, FileShare.Read);
                string ext = Path.GetExtension(result);

                return new ActionResult(fs)
                {
                    ContentType = GetContentType(ext)
                };
            }
            return new ActionResult(new { code = 404, message = "output image not found" }, 404);
        }

        [Route("/image/mods")]
        public ActionResult GetModsImage()
        {
            var result = olsp.GetData("olsp_mod_save_path") as string;
            if (string.IsNullOrEmpty(result)) return new ActionResult(null);

            if(File.Exists(result))
            {
                var fs = File.Open(result, FileMode.Open, FileAccess.Read, FileShare.Read);
                string ext = Path.GetExtension(result);

                return new ActionResult(fs)
                {
                    ContentType = GetContentType(ext)
                };
            }
            return new ActionResult(new { code = 404, message = "mods image not found" }, 404);
        }

        private string GetContentType(string fileExtention)
        {
            switch (fileExtention)
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";

                case ".png":
                    return "image/png";

                default:
                    return "application/octet-stream";
            }
        }
    }
}