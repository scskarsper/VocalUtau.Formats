﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VocalUtau.Formats.Model.BaseObject;

namespace VocalUtau.Formats.Model.VocalObject.ParamTranslater
{
    public class DynCompiler
    {
        Dictionary<long, double> DynCache = new Dictionary<long, double>();
        PartsObject partsObject;
        public DynCompiler(ref PartsObject part)
        {
            this.partsObject = part;
        }
        public void ClearCache()
        {
            DynCache.Clear();
        }
        Object threadLocker = new Object();
        public double getDynValue(long tick)
        {
            if (this.partsObject.DynList.Count == 0)
            {
                this.partsObject.DynList.Add(new TickControlObject(0, 0));
            }
            lock (threadLocker)
            {
                if (DynCache.ContainsKey(tick))
                {
                    return DynCache[tick];
                }
            }
            long newTick = this.partsObject.DynList.FindNearestTick(tick);
            if (newTick != -1)
            {
                lock (threadLocker)
                {
                    DynCache.Add(tick, this.partsObject.DynList.getData(newTick).Value);
                }
                return this.partsObject.DynList.getData(newTick).Value;
            }
            else
            {
                return 0;
            }
        }

        private void clearDyn(ref TickSortList<TickControlObject> dynList, long firstTick, long lastTick)
        {
            if (dynList.Count == 0)
            {
                return;
            }
            dynList.RemvoeTickRange(firstTick, lastTick);
        }
        public void ClearDynLine(long StartTick, long EndTick, bool KeepBound = true)
        {
            ClearCache();
            TickSortList<TickControlObject> PL = partsObject.DynList;

            double stPV = 0, etPV = 0;
            if (KeepBound)
            {
                stPV = getDynValue(StartTick);
                etPV = getDynValue(EndTick);
            }
            clearDyn(ref PL, StartTick, EndTick);
            if (KeepBound)
            {
                partsObject.DynList.Add(new TickControlObject(StartTick, stPV));
                partsObject.DynList.Add(new TickControlObject(EndTick, etPV));
            }
        }
        public void ReplaceDynLine(List<TickControlObject> DynLine, bool KeepBound = true)
        {
            ClearCache();
            List<TickControlObject> poj = DynLine;
            if (poj.Count == 0) return;
            poj.Sort();


            TickSortList<TickControlObject> PL = partsObject.DynList;
            long st = poj[0].Tick;
            long et = poj[poj.Count - 1].Tick;
            if (KeepBound)
            {
                if (KeepBound)
                {
                    poj[0] = new TickControlObject(st, getDynValue(st));

                    poj[poj.Count - 1] = new TickControlObject(et, getDynValue(et));
                }
            }

            clearDyn(ref PL, st, et);
            partsObject.DynList.AddRange(poj);
            partsObject.DynList.Sort();
        }
    }
}
