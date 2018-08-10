using OsuDataDistributeRestful.Server;
using OsuDataDistributeRestful.Server.Api;
using RealTimePPDisplayer;
using RealTimePPDisplayer.Displayer;
using Sync.Plugins;
using System.Collections.Generic;
using System.Linq;

namespace OsuDataDistributeRestful.Api
{
    [Route("/api/rtppd")]
    internal class RtppdApis : IApi
    {
        private RestfulDisplayer[] m_restfuile_displayers = new RestfulDisplayer[16];

        private RealTimePPDisplayerPlugin rtppd;

        public RtppdApis(Plugin rtppd_plugin)
        {
            rtppd = rtppd_plugin as RealTimePPDisplayerPlugin;

            rtppd.RegisterDisplayer("restful", (id) => m_restfuile_displayers[id ?? 0] = new RestfulDisplayer(id));
        }

        [Route("/playingStatus/{id}")]
        public object GetPlayingStatus(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);
            return new
            {
                playing = displayer?.IsPlay
            };
        }

        [Route("/playingStatus")]
        public object GetPlayingStatus()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();
            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => new { playing = d?.IsPlay })
            };
        }

        [Route("/pp/{id}")]
        public object GetPP(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);
            return (displayer?.PPTuple == null) ? null : MakePP(displayer?.PPTuple);
        }

        [Route("/pp")]
        public object GetPP()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.PPTuple == null) ? null : MakePP(d?.PPTuple))
            };
        }

        [Route("/hitCount/{id}")]
        public object GetHitCount(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);

            return (displayer?.HitCountTuple) == null ? null : MakeHitCount(displayer?.HitCountTuple);
        }

        [Route("/hitCount")]
        public object GetHitCount()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.HitCountTuple == null) ? null : MakeHitCount(d?.HitCountTuple))
            };
        }

        [Route("/ppFormat")]
        public object GetPPFormat()
            => new { format = StringFormatter.GetPPFormatter().Format };

        [Route("/hitCountFormat")]
        public object GetHitCountFormat()
            => new { format = StringFormatter.GetHitCountFormatter().Format };

        private object MakePP(PPTuple? tuple)
        {
            return new
            {
                fcppAim = tuple?.FullComboAimPP,
                fcppSpeed = tuple?.FullComboSpeedPP,
                fcppAccuracy = tuple?.FullComboAccuracyPP,
                fcpp = tuple?.FullComboPP,

                rtppAim = tuple?.RealTimeAimPP,
                rtppSpeed = tuple?.RealTimeSpeedPP,
                rtppAccuracy = tuple?.RealTimeAccuracyPP,
                rtpp = tuple?.RealTimePP,

                maxppAim = tuple?.MaxAimPP,
                maxppSpeed = tuple?.MaxSpeedPP,
                maxppAccuracy = tuple?.MaxAccuracyPP,
                maxpp = tuple?.MaxPP,
            };
        }

        private object MakeHitCount(HitCountTuple? tuple)
        {
            return new
            {
                n300g = tuple?.CountGeki,
                n300 = tuple?.Count300,
                n200 = tuple?.CountKatu,
                n150 = tuple?.Count100,
                n100 = tuple?.Count100,
                n50 = tuple?.Count50,
                nmiss = tuple?.CountMiss,

                rtmaxcombo = tuple?.CurrentMaxCombo,
                maxcombo = tuple?.PlayerMaxCombo,
                fullcombo = tuple?.FullCombo,
                combo = tuple?.Combo,
                objectsCount = tuple?.ObjectsCount,

                time = tuple?.PlayTime,
                duration = tuple?.Duration
            };
        }

        private RestfulDisplayer GetDisplayer(int i)
        {
            if (i < 0 || i > 16) return null;

            var displayer = m_restfuile_displayers[i];
            return displayer;
        }

        private List<RestfulDisplayer> EnumerateRestfulDisplayers()
        {
            List<RestfulDisplayer> displayers = new List<RestfulDisplayer>();
            if (rtppd.TourneyMode)
            {
                for (int i = 0; i < rtppd.TourneyWindowCount; i++)
                    displayers.Add(GetDisplayer(i));
            }
            else
                displayers.Add(GetDisplayer(0));

            return displayers;
        }

        private class RestfulDisplayer : DisplayerBase
        {
            public bool IsPlay { get; private set; } = false;
            public HitCountTuple HitCountTuple { get; private set; }
            public PPTuple PPTuple { get; private set; }

            public int ClientID { get; private set; }

            public RestfulDisplayer(int? id)
            {
                ClientID = id ?? 0;
            }

            public override void Clear()
            {
                IsPlay = false;
                HitCountTuple = new HitCountTuple();
            }

            public override void OnUpdateHitCount(HitCountTuple tuple)
            {
                HitCountTuple = tuple;
            }

            public override void OnUpdatePP(PPTuple tuple)
            {
                IsPlay = true;
                PPTuple = tuple;
            }
        }
    }
}