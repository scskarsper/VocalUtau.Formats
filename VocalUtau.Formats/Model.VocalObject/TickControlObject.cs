﻿using System;
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
    public class TickControlObject : IComparable, IComparer<TickControlObject>
    {
        [DataMember]
        public long Tick { get; set; }
        [DataMember]
        public double Value { get; set; }
        public TickControlObject(long Tick, double Value)
        {
            this.Value = Value;
            this.Tick = Tick;
        }
        public int CompareTo(Object o)
        {
            if (this.Tick > ((TickControlObject)o).Tick)
                return 1;
            else if (this.Tick == ((TickControlObject)o).Tick)
                return 0;
            else
                return -1;
        }

        public int Compare(TickControlObject x, TickControlObject y)
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
