using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class PitchObject : IComparable, IComparer<PitchObject>
    {
        [DataMember]
        public long Tick { get; set; }
        public PitchAtomObject.OctaveTypeEnum OctaveType { get { return this.pvp.OctaveType; } set { this.pvp.OctaveType = value; } }
        public PitchObject(long Tick, double PitchValue)
        {
            this.pvp = new PitchAtomObject(PitchValue);
            this.Tick = Tick;
        }
        public PitchObject(long Tick, uint NoteNumber, int PitchWheel)
        {
            this.pvp = new PitchAtomObject(NoteNumber, PitchWheel);
            this.Tick = Tick;
        }
        public PitchObject(long Tick, PitchAtomObject PitchValue)
        {
            this.pvp = PitchValue;
            this.Tick = Tick;
        }
        PitchAtomObject pvp = new PitchAtomObject(60);
        [DataMember]
        public PitchAtomObject PitchValue
        {
            get
            {
                return pvp;
            }
            set
            {
                pvp = value;
            }
        }

        public int CompareTo(Object o)
        {
            if (this.Tick > ((PitchObject)o).Tick)
                return 1;
            else if (this.Tick == ((PitchObject)o).Tick)
                return 0;
            else
                return -1;
        }

        public int Compare(PitchObject x, PitchObject y)
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
