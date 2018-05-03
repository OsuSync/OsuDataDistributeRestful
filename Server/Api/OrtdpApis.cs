using OsuDataDistributeRestful.Server;
using OsuDataDistributeRestful.Server.Api;
using OsuRTDataProvider;
using OsuRTDataProvider.BeatmapInfo;
using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using Sync.Plugins;
using System.IO;
using static OsuRTDataProvider.Listen.OsuListenerManager;

namespace OsuDataDistributeRestful.Api
{
    [Route("/api/ortdp")]
    internal class OrtdpApis : IApi
    {
        private OsuRTDataProviderPlugin ortdp;

        public OrtdpApis(Plugin plugin)
        {
            ortdp = plugin as OsuRTDataProviderPlugin;
        }

        [Route("/gameStatus/{id}")]
        public object GetGameStatus(int id)
        {
            var manager = ortdp.TourneyListenerManagers[id];
            var status = manager?.GetCurrentData((ProvideDataMask)0).Status ?? OsuStatus.Unkonwn;

            return new { status, statusText = status.ToString() };
        }

        [Route("/gameStatus")]
        public object GetGameStatus()
        {
            var manager = ortdp.ListenerManager;
            var status = manager?.GetCurrentData((ProvideDataMask)0).Status ?? OsuStatus.Unkonwn;

            return new { status, statusText = status.ToString() };
        }

        [Route("/gameMode")]
        public object GetGameMode()
        {
            var mode = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.GameMode).PlayMode;

            return new { gameMode = mode, gameModeText = mode.ToString() };
        }

        [Route("/playing/mods/{id}")]
        public object GetPlayingMods(int id)
        {
            var manager = ortdp.TourneyListenerManagers[id];
            var mods = manager?.GetCurrentData(ProvideDataMask.Mods).Mods ?? ModsInfo.Empty;

            return MakeModsInfo(mods);
        }

        [Route("/playing/mods")]
        public object GetPlayingMods()
        {
            var manager = ortdp.ListenerManager;
            var mods = manager?.GetCurrentData(ProvideDataMask.Mods).Mods ?? ModsInfo.Empty;

            return MakeModsInfo(mods);
        }

        [Route("/isTournetMode")]
        public object GetTourneyMode() 
            => new { value = ortdp.TourneyListenerManagers != null };

        [Route("/tournetModeListenCount")]
        public object GetTournetModeListenCount() 
            => new { count = ortdp.TourneyListenerManagersCount };

        [Route("/playing/time/{id}")]
        public object GetPlayingTime(int id)
            => new { time = ortdp.TourneyListenerManagers[id]?.GetCurrentData(ProvideDataMask.Time).Time };

        [Route("/playing/time")]
        public object GetPlayingTime()
            => new { time = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.Time).Time };

        [Route("/beatmap/info/{id}")]
        public object GetBeatmapInfo(int id)
        {
            var beatmap = ortdp.TourneyListenerManagers[id]?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;

            return MakeBeatmap(beatmap);
        }

        [Route("/beatmap/info")]
        public object GetBeatmapInfo()
        {
            var beatmap = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;

            return MakeBeatmap(beatmap);
        }

        [Route("/beatmap")]
        public object GetBeatmap()
        {
            var beatmap = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;

            if(File.Exists(beatmap.FilenameFull))
                return new ActionResult(File.OpenRead(beatmap.FilenameFull)) { ContentType = "text/plain; charset=utf-8" };

            return new ActionResult(new { code = 404,message="no found beatmap file" });
        }

        [Route("/beatmap/audio")]
        public ActionResult GetAudioFile()
        {
            var manager = ortdp.ListenerManager;
            var beatmap = manager?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;
            string filename = Path.Combine(beatmap.Folder, beatmap.AudioFilename);

            if (File.Exists(filename))
            {
                var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);

                return new ActionResult(fs)
                {
                    ContentType = "audio/mpeg"
                };
            }

            return new ActionResult(new { code = 404 },200);
        }

        [Route("/beatmap/backround")]
        public ActionResult GetBackroundFile()
        {
            var manager = ortdp.ListenerManager;
            var beatmap = manager?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;
            string filename = Path.Combine(beatmap.Folder, beatmap.BackgroundFilename);

            if (File.Exists(filename))
            {
                var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
                string ext = Path.GetExtension(filename);

                return new ActionResult(fs)
                {
                    ContentType = GetContentType(ext)
                };
            }

            return new ActionResult(new { code = 404 }, 200);
        }

        //[Route("/beatmap/video")]
        //public ActionResult GetVideoFile()
        //{
        //    var manager = ortdp.ListenerManager;
        //    var beatmap = manager?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;
        //    string filename = Path.Combine(beatmap.Folder, beatmap.VideoFilename);

        //    if (File.Exists(filename))
        //    {
        //        var fs = File.Open(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
        //        string ext = Path.GetExtension(filename);

        //        return new ActionResult(fs)
        //        {
        //            ContentType = GetContentType(ext)
        //        };
        //    }

        //    return new ActionResult(new { code = 404 }, 200);
        //}

        [Route("/playing/info/{id}")]
        public object GetPlayingInfo(int id)
        {
            var info=ortdp.TourneyListenerManagers[id]?.GetCurrentData(
                ProvideDataMask.Score|
                ProvideDataMask.HealthPoint|
                ProvideDataMask.HitCount|
                ProvideDataMask.Combo|
                ProvideDataMask.Accuracy);

            return MakePlayingInfo(info);
        }

        [Route("/playing/info")]
        public object GetPlayingInfo()
        {
            var info = ortdp.ListenerManager.GetCurrentData(
                ProvideDataMask.Score |
                ProvideDataMask.HealthPoint |
                ProvideDataMask.HitCount |
                ProvideDataMask.Combo |
                ProvideDataMask.Accuracy);

            return MakePlayingInfo(info);
        }


        private object MakeModsInfo(ModsInfo mods)
        {
            return new
            {
                timeRate = mods.TimeRate,
                shortName = mods.ShortName,
                name = mods.Name,
                mods = mods.Mod
            };
        }
        private object MakePlayingInfo(ProvideData? info)
        {
            return new
            {
                count300 = info?.Count300,
                count100 = info?.Count100,
                count50 = info?.Count50,
                countMiss = info?.CountMiss,
                countGeki = info?.CountGeki,
                countKatu = info?.CountKatu,
                combo = info?.Combo,
                healthPoint = info?.HealthPoint,
                accuracy = info?.Accuracy,
                score = info?.Score,
            };
        }
        private object MakeBeatmap(Beatmap beatmap)
        {
            return new
            {
                downloadLinkSet = beatmap.DownloadLinkSet,
                downloadLink = beatmap.DownloadLink,
                beatmapSetId = beatmap.BeatmapSetID,
                beatmapId = beatmap.BeatmapID,
                difficulty = beatmap.Difficulty,
                creator = beatmap.Creator,
                artist = beatmap.Artist,
                artistUnicode = beatmap.ArtistUnicode,
                title = beatmap.Title,
                titleUnicode = beatmap.TitleUnicode,

                folder = Path.GetFileName(beatmap.Folder),
                filename = beatmap.Filename,
                audioFilename = beatmap.AudioFilename,
                backroundFilename=beatmap.BackgroundFilename,
                videoFilename=beatmap.VideoFilename
            };
        }

        private string GetContentType(string fileExtention)
        {
            switch (fileExtention.ToLower())
            {
                case ".jpg":case ".jpeg": return "image/jpeg";
                case ".png":return "image/png";
                case ".ogg":return "audio/ogg";

                case ".mp4":return "video/mp4";
                case ".avi":return "video/x-msvideo";
                default:
                    return "application/octet-stream";
            }
        }
    }
}