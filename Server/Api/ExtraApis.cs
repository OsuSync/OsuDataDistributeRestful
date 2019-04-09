using OsuDataDistributeRestful.Server.Api;
using OsuRTDataProvider;
using OsuRTDataProvider.BeatmapInfo;
using OsuRTDataProvider.Listen;
using OsuRTDataProvider.Mods;
using RealTimePPDisplayer;
using RealTimePPDisplayer.Beatmap;
using RealTimePPDisplayer.Calculator;
using Sync.Plugins;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful.Server
{
    [Route("/api/extra")]
    class ExtraApis:IApi
    {
        private OsuRTDataProviderPlugin _ortdp;
        private RealTimePPDisplayerPlugin _rtppd;
        private OsuPlayMode _mode;
        private uint _mods;

        private PerformanceCalculatorBase _stdPpCalculator;
        private PerformanceCalculatorBase _taikoPpCalculator;
        private PerformanceCalculatorBase _maniaPpCalculator;
        private PerformanceCalculatorBase _ctbPpCalculator;

        private Beatmap _beatmap;
        private BeatmapReader _reader;

        public ExtraApis(Plugin ortdp,Plugin rtppd)
        {
            _ortdp = ortdp as OsuRTDataProviderPlugin;
            _rtppd = rtppd as RealTimePPDisplayerPlugin;

            _ortdp.ListenerManager.OnPlayModeChanged += (l, c) => _mode = c;
            _ortdp.ListenerManager.OnModsChanged += (c) => _mods = (uint)c.Mod;
            _ortdp.ListenerManager.OnBeatmapChanged += (b) => _beatmap = b;
        }

        [Route("/fcpp/{acc}")]
        public object GetFcpp(double acc)
        {
            PerformanceCalculatorBase cal = GetCalculator();

            if (cal is null)
            {
                return new ActionResult(null, 500, "The server could not get pp calculator.");
            }

            if (_beatmap is null)
            {
                return new ActionResult(null, 500, "The server could not get beatmap.");
            }

            if (_reader is null ||
                _reader.OrtdpBeatmap.BeatmapID != _beatmap.BeatmapID)
            {
                _reader = new BeatmapReader(_beatmap, (int)_mode);
            }

            if(_reader is null)
            {
                return new ActionResult(null, 500, "The server could not create BeatmapReader.");
            }

            cal.AccuracyRound(acc, _reader.ObjectsCount, 0, out int n300, out int n100, out int n50);

            cal.Mods = _mods;
            cal.Beatmap = _reader;
            cal.Count50 = n50;
            cal.Count100 = n100;
            cal.Count300 = n300;
            cal.CountMiss = 0;

            var pptuple = cal.GetPerformance();

            return new ActionResult(new
            {
                pp = pptuple.FullComboPP,
                aim_pp = pptuple.FullComboAimPP,
                speed_pp = pptuple.FullComboSpeedPP,
                acc_pp = pptuple.FullComboAccuracyPP,
            });
        }

        private PerformanceCalculatorBase GetCalculator()
        {
            switch (_mode)
            {
                case OsuPlayMode.Osu:
                    _stdPpCalculator = _stdPpCalculator ?? new StdPerformanceCalculator();
                    return _stdPpCalculator;
                case OsuPlayMode.Taiko:
                    _taikoPpCalculator = _taikoPpCalculator ?? new TaikoPerformanceCalculator();
                    return _taikoPpCalculator;
                case OsuPlayMode.Mania:
                    _maniaPpCalculator = _maniaPpCalculator ?? new ManiaPerformanceCalculator();
                    return _maniaPpCalculator;
                case OsuPlayMode.CatchTheBeat:
                    _ctbPpCalculator = _ctbPpCalculator ?? new CatchTheBeatPerformanceCalculator();
                    return _ctbPpCalculator;
                default:
                    return null;
            }
        }
    }
}
