using RealTimePPDisplayer;
using RealTimePPDisplayer.Displayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistributeRestful
{
    class RestfulDisplayer : DisplayerBase
    {
        PPTuple m_current_pp;
        PPTuple m_target_pp;
        PPTuple m_speed;

        public bool IsPlay { get; private set; } = false;
        public HitCountTuple HitCountTuple { get; private set; }
        public PPTuple PPTuple => m_target_pp;

        public string StringPP { get; private set; }
        public string StringHitCount { get;private set; }

        public int ClientID { get; private set; }

        public RestfulDisplayer(int? id)
        {
            ClientID = id ?? 0;
        }

        public override void Clear()
        {
            IsPlay = false;
            HitCountTuple = new HitCountTuple();

            m_speed = PPTuple.Empty;
            m_current_pp = PPTuple.Empty;
            m_target_pp = PPTuple.Empty;

            StringPP = string.Empty;
            StringHitCount = string.Empty;
        }

        public override void OnUpdateHitCount(HitCountTuple tuple)
        {
            HitCountTuple = tuple;
            StringHitCount=base.GetFormattedHitCount(tuple).ToString();
        }

        public override void OnUpdatePP(PPTuple tuple)
        {
            IsPlay = true;
            m_target_pp = tuple;
        }

        public override void FixedDisplay(double time)
        {
            if (!IsPlay) return;

            m_current_pp = SmoothMath.SmoothDampPPTuple(m_current_pp, m_target_pp, ref m_speed, time);
            StringPP = base.GetFormattedPP(m_current_pp).ToString();
        }
    }
}
