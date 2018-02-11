using RealTimePPDisplayer.Displayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OsuDataDistribute_Restful
{
    class RestfulDisplayer : DisplayerBase
    {
        public bool IsPlay { get; private set; } = false;
        public HitCountTuple HitCountTuple { get; private set; }
        public PPTuple PPTuple { get; private set; }

        public override void Clear()
        {
            IsPlay = false;
            HitCountTuple = new HitCountTuple();
            PPTuple = PPTuple.Empty;
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
