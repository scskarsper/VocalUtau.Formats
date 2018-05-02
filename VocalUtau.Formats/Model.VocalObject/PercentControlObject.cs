using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class PercentControlObject : IComparable, IComparer<PercentControlObject>
    {
        uint _Percent = 0;

        [DataMember]
        public uint Percent
        {
            get { return _Percent; }
            set { if (value > 100)_Percent = 100;else _Percent = value; }
        }
        [DataMember]
        public double Value { get; set; }
        public PercentControlObject(uint Percent, double Value)
        {
            this.Value = Value;
            this.Percent = Percent;
        }
        public int CompareTo(Object o)
        {
            if (this.Percent > ((PercentControlObject)o).Percent)
                return 1;
            else if (this.Percent == ((PercentControlObject)o).Percent)
                return 0;
            else
                return -1;
        }

        public int Compare(PercentControlObject x, PercentControlObject y)
        {
            if (x.Percent < y.Percent)
                return -1;
            else if (x.Percent == y.Percent)
                return 0;
            else
                return 1;
        }
    }
}
