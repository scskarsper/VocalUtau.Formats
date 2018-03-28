using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.BaseObject;

namespace VocalUtau.Formats.Model.VocalObject.ParamTranslater
{
    public class FastFinderX
    {
        public static int FindPointIndex(long BeFindTick,ref List<PitchObject> PointList, int LeftBound, int RightBound)
        {
            if (LeftBound > RightBound) return -1;
            int mid = (LeftBound + RightBound) / 2;
            if (LeftBound == mid) return LeftBound;
            if (PointList[mid].Tick > BeFindTick) return FindPointIndex(BeFindTick, ref PointList, 0, mid);
            if (PointList[mid].Tick < BeFindTick) return FindPointIndex(BeFindTick, ref PointList, mid, RightBound);
            if (PointList[mid].Tick == BeFindTick) return mid;
            return -1;
        }
        public static int FindPointIndex(long BeFindTick, ref List<ControlObject> PointList, int LeftBound, int RightBound)
        {
            if (LeftBound > RightBound) return -1;
            int mid = (LeftBound + RightBound) / 2;
            if (LeftBound == mid) return LeftBound;
            if (PointList[mid].Tick > BeFindTick) return FindPointIndex(BeFindTick, ref PointList, 0, mid);
            if (PointList[mid].Tick < BeFindTick) return FindPointIndex(BeFindTick, ref PointList, mid, RightBound);
            if (PointList[mid].Tick == BeFindTick) return mid;
            return -1;
        }
    }
}
