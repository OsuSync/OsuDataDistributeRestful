using OsuLiveStatusPanel;
using Sync.Plugins;
using System.IO;

namespace OsuDataDistributeRestful.Olsp
{
    internal class OlspResourceInitializer
    {
        public OlspResourceInitializer(Plugin olsp_plguin, OsuDataDistributeRestfulPlugin oddr)
        {
            OsuLiveStatusPanelPlugin olsp = olsp_plguin as OsuLiveStatusPanelPlugin;

            foreach (var providable_data_name in olsp.EnumProvidableDataName())
            {
                oddr.RegisterResource($"/api/olsp/{providable_data_name}", (param_collection) =>
                {
                    var result = olsp.GetData(providable_data_name);
                    return new
                    {
                        status = result != null,
                        value = result
                    };
                });
            }

            oddr.RegisterResource("/api/olsp/bg_image", (p) =>
             {
                 var result = olsp.GetData("olsp_bg_path") as string;
                 if (string.IsNullOrEmpty(result)) return new StreamResult(null);

                 string ext = Path.GetExtension(result);
                 var fs = File.Open(result, FileMode.Open, FileAccess.Read, FileShare.Read);

                 return new StreamResult(fs)
                 {
                     ContentType = GetContentType(ext)
                 };
             });

            oddr.RegisterResource("/api/olsp/output_bg_image", (p) =>
            {
                var result = olsp.GetData("olsp_bg_save_path") as string;
                if (string.IsNullOrEmpty(result)) return new StreamResult(null);

                string ext = Path.GetExtension(result);
                var fs = File.Open(result, FileMode.Open, FileAccess.Read, FileShare.Read);

                return new StreamResult(fs)
                {
                    ContentType = GetContentType(ext)
                };
            });

            oddr.RegisterResource("/api/olsp/mods_image", (p) =>
            {
                var result = olsp.GetData("olsp_mod_save_path") as string;
                if (string.IsNullOrEmpty(result)) return new StreamResult(null);

                string ext = Path.GetExtension(result);
                var fs = File.Open(result, FileMode.Open, FileAccess.Read, FileShare.Read);

                return new StreamResult(fs)
                {
                    ContentType = GetContentType(ext)
                };
            });
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