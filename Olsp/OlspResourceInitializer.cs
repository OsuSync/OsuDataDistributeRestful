using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OsuLiveStatusPanel;

namespace OsuDataDistributeRestful.Olsp
{
    class OlspResourceInitializer
    {
        public OlspResourceInitializer(Plugin olsp_plguin,OsuDataDistributeRestfulPlugin oddr)
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
        }
    }
}
