using OsuDataDistributeRestful;
using RealTimePPDisplayer;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful.Rtppd
{
    class RtppdResourceInitializer
    {
        private RestfulDisplayer[] m_restfuile_displayers = new RestfulDisplayer[16];

        RestfulDisplayer GetDisplayer(int i)
        {
            if (i < 0 || i > 16) return null;

            var displayer = m_restfuile_displayers[i];
            return displayer;
        }

        public RtppdResourceInitializer(Plugin rtppd_plugin,OsuDataDistributeRestfulPlugin oddr)
        {
            var rtppd = rtppd_plugin as RealTimePPDisplayerPlugin;

            rtppd.RegisterDisplayer("restful", (id) => m_restfuile_displayers[id ?? 0] = new RestfulDisplayer(id));

            oddr.RegisterResource("/api/rtppd/playing", (p) => {
                RestfulDisplayer displayer = GetDisplayer(p.GetInt("client_id") ?? 0);
                return new
                {
                    ClientID = displayer?.ClientID,
                    Playing = displayer?.IsPlay
                };
            });

            oddr.RegisterResource("/api/rtppd/pp", (p) =>
            {
                RestfulDisplayer displayer = GetDisplayer(p.GetInt("client_id") ?? 0);

                return new
                {
                    displayer?.ClientID,
                    Data = displayer?.PPTuple
                };
            });

            oddr.RegisterResource("/api/rtppd/hit_count", (p) =>
            {
                RestfulDisplayer displayer = GetDisplayer(p.GetInt("client_id") ?? 0);

                return new
                {
                    displayer?.ClientID,
                    Data = displayer?.HitCountTuple
                };
            });

            oddr.RegisterResource("/api/rtppd/pp/format", (p) => new { StringFormatter.GetPPFormatter().Format });

            oddr.RegisterResource("/api/rtppd/hit_count/format", (p) => new { StringFormatter.GetHitCountFormatter().Format });

            oddr.RegisterResource("/api/rtppd/pp/formatted_content", (p) => {
                RestfulDisplayer displayer = GetDisplayer(p.GetInt("client_id") ?? 0);
                return new
                {
                    displayer?.ClientID,
                    Content = displayer?.StringPP
                };
            });

            oddr.RegisterResource("/api/rtppd/hit_count/formatted_content", (p) => {
                RestfulDisplayer displayer = GetDisplayer(p.GetInt("client_id") ?? 0);
                return new
                {
                    displayer?.ClientID,
                    Content = displayer?.StringHitCount
                };
            });
        }
    }
}
