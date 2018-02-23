using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class ControlPointObject<T>
    {

        [DataMember]
        SortedDictionary<long, T> sortDic = new SortedDictionary<long, T>();

        public void Add(long Tick, T data)
        {
            long sTick = MathUtils.GetNormalizeTick(Tick);
            if (sortDic.ContainsKey(sTick))
            {
                sortDic[sTick] = data;
            }
            else
            {
                sortDic.Add(sTick, data);
            }
        }
        public void Del(long Tick)
        {
            long sTick = MathUtils.GetNormalizeTick(Tick);
            if (sortDic.ContainsKey(sTick))
            {
                sortDic.Remove(sTick);
            }
        }
        public T GetValue(long Tick,T defaultValue=default(T))
        {
            long sTick = MathUtils.GetNormalizeTick(Tick);
            if (sortDic.ContainsKey(sTick))
            {
                return sortDic[Tick];
            }
            return defaultValue;
        }
        public void ClearRange(long startTick, long endTick)
        {
            long[] ke=sortDic.Keys.ToArray();
            long st=0;
            while (st < ke.Length && ke[st] < startTick)
            {
                st++;
            }
            while (st < ke.Length && ke[st] >= startTick && ke[st] <= endTick)
            {
                sortDic.Remove(ke[st]);
                st++;
            }
        }
    }
}
