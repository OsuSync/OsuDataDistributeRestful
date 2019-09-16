using OsuDataDistributeRestful.Server;
using OsuDataDistributeRestful.Server.Api;
using RealTimePPDisplayer;
using RealTimePPDisplayer.Displayer;
using RealTimePPDisplayer.Formatter;
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
            return (displayer?.Pp == null) ? null : MakeTupleResult(displayer?.Pp);
        }

        [Route("/pp")]
        public object GetPP()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.Pp == null) ? null : MakeTupleResult(d?.Pp))
            };
        }

        [Route("/hitCount/{id}")]
        public object GetHitCount(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);

            return (displayer?.HitCount) == null ? null : MakeTupleResult(displayer?.HitCount);
        }

        [Route("/hitCount")]
        public object GetHitCount()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.HitCount == null) ? null : MakeTupleResult(d?.HitCount))
            };
        }

        [Route("/beatmapTuple/{id}")]
        public object GetBeatmap(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);

            return (displayer?.HitCount) == null ? null : MakeTupleResult(displayer?.BeatmapTuple);
        }

        [Route("/beatmapTuple")]
        public object GetBeatmap()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => (d?.HitCount == null) ? null : MakeTupleResult(d?.BeatmapTuple))
            };
        }

        [Route("/ppFormat")]
        public object GetPPFormat()
            => new {format = RtppFormatter.GetPPFormatter().Format};

        [Route("/hitCountFormat")]
        public object GetHitCountFormat()
            => new {format = RtppFormatter.GetHitCountFormatter().Format };

        [Route("/formatted/pp")]
        public object GetFormatedPP()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();
            string format = FormatterBase.GetPPFormatter().Format;
            return new
            {
                count = displayers.Count,
                list = displayers.Select(d=>format)
            };
        }

        [Route("/formatted/pp/{id}")]
        public object GetFormatedPP(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);
            return FormatterBase.GetPPFormatter().Format;
        }

        [Route("/formatted/hitCount/{id}")]
        public object GetFormatedHitCount(int id)
        {
            RestfulDisplayer displayer = GetDisplayer(id);
            return FormatterBase.GetHitCountFormatter().Format;
        }

        [Route("/formatted/hitCount")]
        public object GetFormatedHitCount()
        {
            List<RestfulDisplayer> displayers = EnumerateRestfulDisplayers();
            string format = FormatterBase.GetHitCountFormatter().Format;

            return new
            {
                count = displayers.Count,
                list = displayers.Select(d => format)
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

            public RestfulDisplayer(int? id):base(id)
            {
            }

            public override void Clear()
            {
                base.Clear();
                IsPlay = false;
            }

            public override void Display()
            {
                IsPlay = true;
            }
        }
    }
}