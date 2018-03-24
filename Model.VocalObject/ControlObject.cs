using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VocalUtau.Formats.Model.BaseObject;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class ControlObject : IComparable, IComparer<ControlObject>, ITickSortAtom<ControlObject>
    {
        [DataMember]
        public long Tick { get; set; }
        [DataMember]
        public double Value { get; set; }
        public ControlObject(long Tick, double Value)
        {
            this.Value = Value;
            this.Tick = Tick;
        }
        public long getTick()
        {
            return this.Tick;
        }
        public void setTick(long value)
        {
            this.Tick = value;
        }
        public ControlObject getThis()
        {
            return this;
        }
        public int CompareTo(Object o)
        {
            if (this.Tick > ((ControlObject)o).Tick)
                return 1;
            else if (this.Tick == ((ControlObject)o).Tick)
                return 0;
            else
                return -1;
        }

        public int Compare(ControlObject x, ControlObject y)
        {
            if (x.Tick < y.Tick)
                return -1;
            else if (x.Tick == y.Tick)
                return 0;
            else
                return 1;
        }  
    }
}
