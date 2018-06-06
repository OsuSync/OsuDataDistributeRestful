using OsuDataDistributeRestful.Server;
using OsuDataDistributeRestful.Server.Api;
using OsuRTDataProvider;
using OsuRTDataProvider.BeatmapInfo;
using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.IO;

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
            dynamic status = GetGameStatus();

            if (id < status.list.Count)
                return status.list[id];
            else
                return null;
        }

        [Route("/gameStatus")]
        public object GetGameStatus() =>
            MakeProvideDatas((ProvideDataMask)0, data =>
             new { status = data.Status, statusText = data.Status.ToString() });

        [Route("/gameMode")]
        public object GetGameMode()
        {
            var mode = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.GameMode).PlayMode;

            return new { gameMode = mode, gameModeText = mode.ToString() };
        }

        [Route("/isTournetMode")]
        public object GetTourneyMode()
            => new { value = ortdp.TourneyListenerManagers != null };

        [Route("/tournetModeListenCount")]
        public object GetTournetModeListenCount()
            => new { count = ortdp.TourneyListenerManagersCount };

        #region Beatmap

        [Route("/beatmap/info")]
        public object GetBeatmapInfo() =>
            MakeBeatmap(ortdp.ListenerManager.GetCurrentData(ProvideDataMask.Beatmap).Beatmap);

        [Route("/beatmap")]
        public object GetBeatmap()
        {
            var beatmap = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;

            if (File.Exists(beatmap.FilenameFull))
                return new ActionResult(File.OpenRead(beatmap.FilenameFull)) { ContentType = "text/plain; charset=utf-8" };

            return new ActionResult(new { code = 404, message = "no found beatmap file" });
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

            return new ActionResult(new { code = 404 }, 200);
        }

        [Route("/beatmap/background")]
        public ActionResult GetBackgroundFile()
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

        [Route("/beatmap/video")]
        public ActionResult GetVideoFile()
        {
            var manager = ortdp.ListenerManager;
            var beatmap = manager?.GetCurrentData(ProvideDataMask.Beatmap).Beatmap;
            string filename = Path.Combine(beatmap.Folder, beatmap.VideoFilename);

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

        #endregion Beatmap

        #region Playing

        [Route("/playing/info/{id}")]
        public object GetPlayingInfo(int id)
        {
            dynamic infos = GetPlayingInfo();

            if (id < infos.list.Count)
                return infos.list[id];
            else
                return null;
        }

        [Route("/playing/info")]
        public object GetPlayingInfo() =>
            MakeProvideDatas(
                ProvideDataMask.Score |
                ProvideDataMask.HealthPoint |
                ProvideDataMask.HitCount |
                ProvideDataMask.Combo |
                ProvideDataMask.Accuracy,
                (data) => MakePlayingInfo(data));

        [Route("/playing/mods/{id}")]
        public object GetPlayingMods(int id)
        {
            dynamic mods = GetPlayingMods();

            if (id < mods.list.Count)
                return mods.list[id];
            else
                return null;
        }

        [Route("/playing/mods")]
        public object GetPlayingMods()
            => MakeProvideDatas(ProvideDataMask.Mods, data => MakeModsInfo(data.Mods));

        [Route("/playing/time")]
        public object GetPlayingTime()
            => new { time = ortdp.ListenerManager.GetCurrentData(ProvideDataMask.Time).Time };

        #endregion Playing

        #region tools

        private object MakeProvideDatas(ProvideDataMask mask, Func<ProvideData, object> selector)
        {
            bool isTourney = ortdp.TourneyListenerManagersCount != 0;

            var ret = new
            {
                tourneyMode = isTourney,
                count = isTourney ? ortdp.TourneyListenerManagersCount : 1,
                list = new List<object>()
            };

            if (isTourney)
            {
                foreach (var manager in ortdp.TourneyListenerManagers)
                {
                    var data = manager.GetCurrentData(mask);
                    ret.list.Add(selector(data));
                }
            }
            else
            {
                var manager = ortdp.ListenerManager;
                var data = manager.GetCurrentData(mask);
                ret.list.Add(selector(data));
            }

            return ret;
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
                backroundFilename = beatmap.BackgroundFilename,
                videoFilename = beatmap.VideoFilename
            };
        }

        private string GetContentType(string fileExtention)
        {
            switch (fileExtention.ToLower())
            {
                case ".jpg": case ".jpeg": return "image/jpeg";
                case ".png": return "image/png";
                case ".ogg": return "audio/ogg";

                case ".mp4": return "video/mp4";
                case ".avi": return "video/x-msvideo";
                default:
                    return "application/octet-stream";
            }
        }

        #endregion tools
    }
}