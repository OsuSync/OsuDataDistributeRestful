using OsuDataDistributeRestful.Server;
using OsuDataDistributeRestful.Server.Api;
using RealTimePPDisplayer;
using RealTimePPDisplayer.Displayer;
using Sync.Plugins;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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
            return (displayer?.PPTuple == null) ? null : MakeTupleResult(displayer?.PPTuple);
        }

        [Route("/pp")]
        public object GetPP()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.PPTuple == null) ? null : MakeTupleResult(d?.PPTuple))
            };
        }

        [Route("/hitCount/{id}")]
        public object GetHitCount(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);

            return (displayer?.HitCountTuple) == null ? null : MakeTupleResult(displayer?.HitCountTuple);
        }

        [Route("/hitCount")]
        public object GetHitCount()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.HitCountTuple == null) ? null : MakeTupleResult(d?.HitCountTuple))
            };
        }

        [Route("/beatmap/{id}")]
        public object GetBeatmap(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);

            return (displayer?.HitCountTuple) == null ? null : MakeTupleResult(displayer?.BeatmapTuple);
        }

        [Route("/beatmap")]
        public object GetBeatmap()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.HitCountTuple == null) ? null : MakeTupleResult(d?.BeatmapTuple))
            };
        }

        [Route("/ppFormat")]
        public object GetPPFormat()
            => new {format = StringFormatter.GetPPFormatter().Format};

        [Route("/hitCountFormat")]
        public object GetHitCountFormat()
            => new {format = StringFormatter.GetHitCountFormatter().Format};

        [Route("/formated/pp")]
        public object GetFormatedPP()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d=>d.FormatPp())
            };
        }

        [Route("/formated/pp/{0}")]
        public object GetFormatedPP(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);
            return displayer.FormatPp();
        }

        [Route("/formated/hitCount/{0}")]
        public object GetFormatedHitCount(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);
            return displayer.FormatHitCount();
        }

        [Route("/formated/hitCount")]
        public object GetFormatedHitCount()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => d.FormatHitCount())
            };
        }

        private object MakeTupleResult<T>(T? tuple) where T:struct
        {
            Dictionary<string,object> dict = new Dictionary<string, object>();
            T ntuple = tuple.Value;

            foreach (var field in typeof(T).GetFields(BindingFlags.GetField|BindingFlags.Instance|BindingFlags.Public))
            {
                string name = field.Name;
                string name1 = name.Substring(0, 1).ToLower();
                string name2 = name.Substring(1);
                dict.Add(name1+name2,field.GetValue(ntuple));
            }

            return dict;
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

            public override void Display()
            {
                IsPlay = true;
                PPTuple = Pp;
                HitCountTuple = HitCount;
            }
        }
    }
}