using OsuRTDataProvider;
using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static OsuRTDataProvider.Listen.OsuListenerManager;

namespace OsuDataDistributeRestful.Ortdp
{
    class OrtdpResourceInitializer
    {
        public OrtdpResourceInitializer(Plugin plugin, OsuDataDistributeRestfulPlugin oddr)
        {
            var ortdp = plugin as OsuRTDataProviderPlugin;
            oddr.RegisterResource("/api/ortdp/tourney_mode", (p) => new { value = ortdp.TourneyListenerManagers != null });
            oddr.RegisterResource("/api/ortdp/listener_count", (p) => new { count = ortdp.TourneyListenerManagersCount });

            oddr.RegisterResource("/api/ortdp/{id}/playing_time", (p) =>
             {
                 var manager = GetListenManager(ortdp, p);
                 return new { playing_time = manager?.GetCurrentData(ProvideDataMask.Time).Time };
             });

            oddr.RegisterResource("/api/ortdp/{id}/beatmap", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                var beatmap = manager?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;

                return new
                {
                    download_link_set = beatmap.DownloadLinkSet,
                    download_link = beatmap.DownloadLink,
                    beatmap_set_id = beatmap.BeatmapSetID,
                    beatmap_id = beatmap.BeatmapID,
                    difficulty = beatmap.Difficulty,
                    creator = beatmap.Creator,
                    artist = beatmap.Artist,
                    artist_unicode=beatmap.ArtistUnicode,
                    title = beatmap.Title,
                    title_unicode = beatmap.TitleUnicode,

                    folder = Path.GetFileName(beatmap.Folder),
                    filename = beatmap.Filename,
                    audio_filename = beatmap.AudioFilename
                };
            });

            oddr.RegisterResource("/api/ortdp/audio_file",(p)=>
            {
                var manager = ortdp.ListenerManager;
                var beatmap = manager?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;
                string filename = Path.Combine(beatmap.Folder, beatmap.AudioFilename);

                var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                return new StreamResult(fs)
                {
                    ContentType="audio/mpeg"
                };
            });

            oddr.RegisterResource("/api/ortdp/{id}/hit_count", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                var hitcount = manager?.GetCurrentData(ProvideDataMask.HitCount|ProvideDataMask.Combo);

                return new
                {
                    count_300 = hitcount?.Count300,
                    count_100 = hitcount?.Count100,
                    count_50 = hitcount?.Count50,
                    count_miss = hitcount?.CountMiss,
                    count_geki = hitcount?.CountGeki,
                    count_katu = hitcount?.CountKatu,
                    combo = hitcount?.Combo
                };
            });

            oddr.RegisterResource("/api/ortdp/{id}/hp", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                double hp = manager?.GetCurrentData(ProvideDataMask.HealthPoint).HealthPoint??0;

                return new{hp};
            });

            oddr.RegisterResource("/api/ortdp/{id}/acc", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                double acc = manager?.GetCurrentData(ProvideDataMask.Accuracy).HealthPoint ?? 0;

                return new{acc};
            });

            oddr.RegisterResource("/api/ortdp/{id}/game_mode", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                var mode = manager?.GetCurrentData(ProvideDataMask.GameMode).PlayMode;

                return new { game_mode = mode,game_mode_text=mode.ToString()};
            });

            oddr.RegisterResource("/api/ortdp/{id}/mods", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                var mods = manager?.GetCurrentData(ProvideDataMask.Mods).Mods??ModsInfo.Empty;

                return new
                {
                    time_rate=mods.TimeRate,
                    short_name=mods.ShortName,
                    name=mods.Name,
                    mods=mods.Mod
                };
            });

            oddr.RegisterResource("/api/ortdp/{id}/score", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                var score = manager?.GetCurrentData(ProvideDataMask.Score).Score??0;

                return new{score};
            });

            oddr.RegisterResource("/api/ortdp/{id}/game_status", (p) =>
            {
                var manager = GetListenManager(ortdp, p);
                var status = manager?.GetCurrentData((ProvideDataMask)0).Status?? OsuStatus.Unkonwn;

                return new { status,status_text=status.ToString()};
            });
        }

        private OsuListenerManager GetListenManager(OsuRTDataProviderPlugin ortdp, ParamCollection p)
        {
            OsuListenerManager manager=ortdp.ListenerManager;

            if (int.TryParse(p["id"], out int id))
            {
                if (ortdp.TourneyListenerManagers!=null&&
                    id >= 0 && id < ortdp.TourneyListenerManagersCount)
                {
                    if (ortdp.TourneyListenerManagers != null)
                        manager = ortdp.TourneyListenerManagers[id];
                }
            }

            return manager;
        }
    }
}
