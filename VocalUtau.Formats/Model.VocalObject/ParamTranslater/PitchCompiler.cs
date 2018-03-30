using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VocalUtau.Formats.Model.BaseObject;

namespace VocalUtau.Formats.Model.VocalObject.ParamTranslater
{
    public class PitchCompiler
    {
        //ISSUE：PITCH该问Dictionary存储，加快检索和删除速度
        Dictionary<long, double> BaseCache = new Dictionary<long, double>();
        Dictionary<long, double> PitchCache = new Dictionary<long, double>();
        public void ClearCache()
        {
            BaseCache.Clear();
            PitchCache.Clear();
        }

        PartsObject partsObject;
        public PitchCompiler(ref PartsObject part)
        {
            this.partsObject = part;
        }
        public void InitPitchBase()
        {
            FixedPitch();
            SetupBasePitch();
        }
        public void FixedPitch()
        {
            if (partsObject.PitchList.Count == 0) partsObject.PitchList.Add(new PitchObject(0, 0));
            ClearCache();
        }
        public void SetupBasePitch_Aysnc(AsyncWorkCallbackHandler CallBack, int NoteStartIndex = 0, int NoteEndIndex = -1)
        {
            Task.Factory.StartNew(() =>
            {
                SetupBasePitch(NoteStartIndex, NoteEndIndex);
                if (CallBack != null) CallBack(NoteStartIndex, NoteEndIndex);
            });
        }
        public delegate void AsyncWorkCallbackHandler(int NoteStartIndex, int NoteEndIndex);
        private long GetNoteEnd(int NoteIndex)
        {
            long EndTick = partsObject.NoteList[NoteIndex].Tick + partsObject.NoteList[NoteIndex].Length;
            if (NoteIndex < partsObject.NoteList.Count-1)
            {
                if (EndTick > GetNoteStart(NoteIndex + 1))
                {
                    EndTick = GetNoteStart(NoteIndex + 1) - 1;
                }
            }
            return EndTick;
        }
        private long GetNoteStart(int NoteIndex)
        {
            long StartTick=partsObject.NoteList[NoteIndex].Tick;
            return StartTick;
        }
        public double GetNotePitchValue(int NoteIndex)
        {
            return partsObject.NoteList[NoteIndex].PitchValue.PitchValue;
        }
        public void SetupBasePitch(int NoteStartIndex = 0, int NoteEndIndex = -1)
        {
            if (partsObject.BasePitchList == null) partsObject.BasePitchList = new TickSortList<PitchObject>();
            if (partsObject.NoteList.Count == 0) { partsObject.BasePitchList.Clear(); return; }
            if (NoteEndIndex < 0) NoteEndIndex = partsObject.NoteList.Count - 1;
            if (NoteStartIndex < 0) NoteStartIndex = 0;
            if (NoteEndIndex > partsObject.NoteList.Count - 1) NoteEndIndex = partsObject.NoteList.Count - 1;
            
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Console.WriteLine("BasePitch:{0},{1}", NoteStartIndex, NoteEndIndex);
            Console.WriteLine("Start:{0}", watch.Elapsed);

            //CalcAreaStartEnd
            long FirstTick = 0;
            long AsTick = 0;
            if (NoteStartIndex > 0)
            {
//                NoteObject firstObj = partsObject.NoteList[NoteStartIndex - 1];
                AsTick = GetNoteEnd(NoteStartIndex - 1);
                FirstTick = AsTick - calcTransLength(NoteStartIndex - 1, true);
            }
            long LastTick = partsObject.NoteList[partsObject.NoteList.Count - 1].Tick + partsObject.NoteList[partsObject.NoteList.Count - 1].Length;
            long EsTick = LastTick;
            if (NoteEndIndex < partsObject.NoteList.Count - 1)
            {
                EsTick = GetNoteEnd(NoteEndIndex);
                LastTick = EsTick - calcTransLength(NoteEndIndex, true);
            }
            TickSortList<PitchObject> BPL = partsObject.BasePitchList;
            Console.WriteLine("BeforeClearAreaed:{0}", watch.Elapsed);
            clearPitchList(ref BPL, FirstTick, LastTick);
            Console.WriteLine("ClearAreaed:{0}", watch.Elapsed);

            for(int i=NoteStartIndex;i<=NoteEndIndex;i++)
            {
                if (i == NoteStartIndex)
                {
                    if (NoteStartIndex == 0)
                    {
                        NoteObject curObj = partsObject.NoteList[0];
                        long StartDirTick = GetNoteStart(0) + calcTransLength(0, false);
                        partsObject.BasePitchList.Add(new PitchObject(0, curObj.PitchValue.PitchValue));
                    }
                    else
                    {
                        AddTransPart(NoteStartIndex - 1, NoteStartIndex, true);
                    }
                }
                else
                {
                    AddTransPart(i - 1, i);
                }

                if (i == NoteEndIndex)
                {
                    if (NoteEndIndex != partsObject.NoteList.Count - 1)
                    {
                        AddTransPart(NoteEndIndex, NoteEndIndex + 1, true);
                    }
                }
            };       
            Console.WriteLine("FilledLine:{0}", watch.Elapsed);
            if (partsObject.BasePitchList.Count <= 0)
            {
                partsObject.BasePitchList.Add(new PitchObject(0, 60));
            }
            else if(partsObject.BasePitchList[0].Tick!=0)
            {
                partsObject.BasePitchList.Add(new PitchObject(0, partsObject.BasePitchList[0].PitchValue.PitchValue));
            }
            //防止头部出错
            if (NoteStartIndex!=0 && partsObject.NoteList.Count > 0 && partsObject.BasePitchList[0].PitchValue.PitchValue != partsObject.NoteList[0].PitchValue.PitchValue)
            {
                SetupBasePitch(0, 0);
                return;
            }
            //防止尾巴部出错
            if (NoteEndIndex != partsObject.NoteList.Count - 1 && partsObject.NoteList.Count > 0 && partsObject.BasePitchList[partsObject.BasePitchList.Count - 1].PitchValue.PitchValue != partsObject.NoteList[partsObject.NoteList.Count - 1].PitchValue.PitchValue)
            {
                SetupBasePitch(partsObject.NoteList.Count - 1, partsObject.NoteList.Count - 1);
                return;
            }
            ClearCache();
            Console.WriteLine("End:{0}", watch.Elapsed);
            watch.Stop();
        }
        private void AddTransPart(int preObjIndex, int nxtObjIndex,bool ClearBefore=false)
        {
            long CutDown = 480;//超过这个间隔的不计算

            long PNE = GetNoteEnd(preObjIndex);
            long TStart = PNE - calcTransLength(preObjIndex, true);
            long NNS = GetNoteStart(nxtObjIndex);
            long TEnd = NNS + calcTransLength(nxtObjIndex, false);
            if (TStart < TEnd)
            {
                if (ClearBefore)
                {
                    TickSortList<PitchObject> BPL = partsObject.BasePitchList;
                    clearPitchList(ref BPL, TStart, TEnd);
                }
                if (Math.Abs(PNE - NNS) > 480)
                {
                    List<PitchObject> tmp = new List<PitchObject>();
                    double p1=GetNotePitchValue(preObjIndex);
                    double p2 = GetNotePitchValue(nxtObjIndex);
                    tmp.Add(new PitchObject(TStart, p1));
                    tmp.Add(new PitchObject(PNE, p1));
                    tmp.Add(new PitchObject(NNS, p2));
                    tmp.Add(new PitchObject(TEnd, p2));
                    partsObject.BasePitchList.AddRange(tmp);
                }
                else
                {
                    partsObject.BasePitchList.AddRange(CalcGraphS(new PitchObject(TStart, GetNotePitchValue(preObjIndex)), new PitchObject(TEnd, GetNotePitchValue(nxtObjIndex))));
                }
            }
            else
            {
                long TPEnd = GetNoteEnd(nxtObjIndex) - calcTransLength(nxtObjIndex, true);
                TEnd = TStart + 30;
                if (TEnd > TPEnd) TEnd = TPEnd;
                if (ClearBefore)
                {
                    TickSortList<PitchObject> BPL = partsObject.BasePitchList;
                    clearPitchList(ref BPL, TStart, TPEnd);
                }
                if (Math.Abs(PNE - NNS) > 480)
                {
                    List<PitchObject> tmp = new List<PitchObject>();
                    double p1 = GetNotePitchValue(preObjIndex);
                    double p2 = GetNotePitchValue(nxtObjIndex);
                    tmp.Add(new PitchObject(TStart, p1));
                    tmp.Add(new PitchObject(PNE, p1));
                    tmp.Add(new PitchObject(NNS, p2));
                    tmp.Add(new PitchObject(TEnd, p2));
                    partsObject.BasePitchList.AddRange(tmp);
                }
                else
                {
                    partsObject.BasePitchList.AddRange(CalcGraphS(new PitchObject(TStart, GetNotePitchValue(preObjIndex)), new PitchObject(TEnd, GetNotePitchValue(nxtObjIndex))));
                }
                for (long i = TEnd+1; i <= TPEnd; i++)
                {
                    partsObject.BasePitchList.Add(new PitchObject(i, GetNotePitchValue(nxtObjIndex)));
                }
            
            }

            
        }
        private List<PitchObject> CalcGraphS(PitchObject S1, PitchObject S2)
        {
            if (S1 == null) return new List<PitchObject>();
            if (S2 == null) return new List<PitchObject>();
            //系数计算
            //0点坐标
            double B = 3.1415926 / (((double)S2.Tick - (double)S1.Tick));
            double C = -B * S1.Tick;
            double A = Math.Abs(S1.PitchValue.PitchValue - S2.PitchValue.PitchValue) / 2;

            List<PitchObject> ret = new List<PitchObject>();
            for (long i = TickSortList<PitchObject>.TickFormat(Math.Min(S1.Tick, S2.Tick)); i <= TickSortList<PitchObject>.TickFormat(Math.Max(S1.Tick, S2.Tick)); i = i + TickSortList<PitchObject>.TickStep)
            {
                if (S2.PitchValue.PitchValue <= S1.PitchValue.PitchValue)
                {
                    ret.Add(new PitchObject(i, S2.PitchValue.PitchValue + A + A * Math.Cos(B * i + C)));
                }
                else
                {
                    ret.Add(new PitchObject(i, S1.PitchValue.PitchValue + A - A * Math.Cos(B * i + C)));
                }
            }
            return ret;
        }
        private void clearPitchList(ref TickSortList<PitchObject> pitchList, long firstTick, long lastTick)
        {
            if (pitchList.Count == 0)
            {
                return;
            }
            pitchList.RemvoeTickRange(firstTick, lastTick);
        }
        private long calcTransLength(int NoteIndex,bool isTail = false)
        {
            //NoteObject obj = partsObject.NoteList[NoteIndex];
            long Length = GetNoteEnd(NoteIndex) - GetNoteStart(NoteIndex);
            if (Length < 0) Length = 0;
            if (!isTail)
            {
                long PreLen = (long)((double)Length * 0.3);
                if (PreLen > 120)
                {
                    PreLen = 120;
                }
                else if (Length <= 120)
                {
                    PreLen = Length / 2;
                }
                return PreLen;
            }
            else
            {
                long NxtLen = (long)((double)Length * 0.3);
                if (NxtLen > 120)
                {
                    NxtLen = 120;
                }
                else if (Length <= 120)
                {
                    NxtLen = Length / 2;
                }
                return NxtLen;
            }
        }
        Object ReaderthreadLocker = new object();
        public double getRealPitch(long tick)
        {
            double ret = 0;
            lock (ReaderthreadLocker)
            {
                ret=getBasePitch(tick) + getPitch(tick);
            }
            return ret;
        }
        public double getBasePitch(long tick)
        {
            if (BaseCache.ContainsKey(tick))
            {
                return BaseCache[tick];
            }
            /*
            List<PitchObject> pol = partsObject.BasePitchList;
            int i = FastFinder.FindPointIndex(tick, ref pol, 0, pol.Count);
            double value = 0;
            if (i >= 0)
            {
                value = pol[i].PitchValue.PitchValue;
            }*/
            long newTick=partsObject.BasePitchList.FindNearestTick(tick);
            if (newTick != -1)
            {
                BaseCache.Add(tick, partsObject.BasePitchList.getData(newTick).PitchValue.PitchValue);
                return partsObject.BasePitchList.getData(newTick).PitchValue.PitchValue;
            }
            else
            {
                return 0;
            }
        }
        Object threadLocker=new Object();
        public double getPitch(long tick)
        {
            if (this.partsObject.PitchList.Count == 0)
            {
                this.partsObject.PitchList.Add(new PitchObject(0, 0));
            }
            if (PitchCache.ContainsKey(tick))
            {
                return PitchCache[tick];
            }
            long newTick = this.partsObject.PitchList.FindNearestTick(tick);
            if (newTick != -1)
            {
                lock (threadLocker)
                {
                    PitchCache.Add(tick, this.partsObject.PitchList.getData(newTick).PitchValue.PitchValue);
                }
                return this.partsObject.PitchList.getData(newTick).PitchValue.PitchValue;
            }
            else
            {
                return 0;
            }
        }
        public void ClearPitchLine(long StartTick, long EndTick, bool KeepBound = true)
        {
            ClearCache();
            TickSortList<PitchObject> PL = partsObject.PitchList;

            double stPV=0, etPV=0;
            if (KeepBound)
            {
                stPV = getPitch(StartTick);
                etPV = getPitch(EndTick);
            }
            clearPitchList(ref PL, StartTick, EndTick);
            if (KeepBound)
            {
                partsObject.PitchList.Add(new PitchObject(StartTick, stPV));
                partsObject.PitchList.Add(new PitchObject(EndTick, etPV));
            }
        }
        public void ReplacePitchLine(List<PitchObject> PitchBend,bool KeepBound=true)
        {
            ClearCache();
            List<PitchObject> poj = PitchBend;
            if (poj.Count == 0) return;
            poj.Sort();

            
            TickSortList<PitchObject> PL = partsObject.PitchList;
            long st = poj[0].Tick;
            long et = poj[poj.Count - 1].Tick;
            if (KeepBound)
            {
                if (KeepBound)
                {
                    poj[0] = new PitchObject(st, getPitch(st));

                    poj[poj.Count - 1] = new PitchObject(et, getPitch(et));
                }
            }

            clearPitchList(ref PL, st, et);
            partsObject.PitchList.AddRange(poj);
            partsObject.PitchList.Sort();
        }
        public void ReplaceRealPitchLine(List<PitchObject> PitchBend, bool KeepBound = true)
        {
            ClearCache();
            List<PitchObject> poj = PitchBend;
            if (poj.Count == 0) return;
            long st = poj[0].Tick;
            long et = poj[poj.Count - 1].Tick;

            TickSortList<PitchObject> PL = partsObject.PitchList;
            if (KeepBound)
            {
                if (KeepBound)
                {
                    poj[0] = new PitchObject(st, getPitch(st) + getBasePitch(st));

                    poj[poj.Count - 1] = new PitchObject(et, getPitch(et) + getBasePitch(et));
                }
            }

            clearPitchList(ref PL, st, et);
            for(int i=0;i<poj.Count;i++)
            {
                long tick = poj[i].Tick;
                double value = poj[i].PitchValue.PitchValue;
                value = value - getBasePitch(tick);
                partsObject.PitchList.Add(new PitchObject(tick, value));
            };
            partsObject.PitchList.Sort();
        }

    }
}
