using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class NoteObject : IComparable, IComparer<NoteObject>, ICloneable
    {
        /// <summary>
        /// INIT 初始化
        /// </summary>
        #region
        public PitchAtomObject.OctaveTypeEnum OctaveType { get { return this.pvp.OctaveType; } set { this.pvp.OctaveType = value; } }

        public NoteObject(long Tick, long Length, double PitchValue)
        {
            this.pvp = new PitchAtomObject(PitchValue);
            this.Tick = Tick;
            this.Length = Length;
        }
        public NoteObject(long Tick, long Length, uint NoteNumber, int PitchWheel)
        {
            this.pvp = new PitchAtomObject(NoteNumber, PitchWheel);
            this.Tick = Tick;
            this.Length = Length;
        }
        public NoteObject(long Tick, long Length, PitchAtomObject PitchValue)
        {
            this.pvp = PitchValue;
            this.Tick = Tick;
            this.Length = Length;
        }

        public NoteObject(long Tick, long Length, double PitchValue, string Lyric)
        {
            this.pvp = new PitchAtomObject(PitchValue);
            this.Tick = Tick;
            this.Length = Length;
            this.Lyric = Lyric;
        }
        public NoteObject(long Tick, long Length, uint NoteNumber, int PitchWheel, string Lyric)
        {
            this.pvp = new PitchAtomObject(NoteNumber, PitchWheel);
            this.Tick = Tick;
            this.Length = Length;
            this.Lyric = Lyric;
        }
        public NoteObject(long Tick, long Length, PitchAtomObject PitchValue, string Lyric)
        {
            this.pvp = PitchValue;
            this.Tick = Tick;
            this.Length = Length;
            this.Lyric = Lyric;
        }

        #endregion

        [DataMember]
        public long Tick { get; set; }
        [DataMember]
        public long Length { get; set; }
        string _lyc = "";
        [DataMember]
        public string Lyric
        {
            get { return _lyc; }
            set
            {
                _lyc = value;
                if (_PhonemeAtoms == null)
                {
                    _PhonemeAtoms = new List<NoteAtomObject>() { new NoteAtomObject() };
                }
                if (_PhonemeAtoms.Count == 1)
                {
                    if (_PhonemeAtoms[0].PhonemeAtom == "")
                    {
                        _PhonemeAtoms[0].PhonemeAtom = _lyc;
                    }
                }
            }
        }
        PitchAtomObject pvp = new PitchAtomObject(60);
        [IgnoreDataMember]
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

        [DataMember]
        public double Pitch
        {
            get
            {
                return pvp.PitchValue;
            }
            set
            {
                pvp = new PitchAtomObject(value);
            }
        }

        List<NoteAtomObject> _PhonemeAtoms = new List<NoteAtomObject>() { new NoteAtomObject() };

        [DataMember]
        public List<NoteAtomObject> PhonemeAtoms
        {
            get
            {
                if (_PhonemeAtoms == null)
                {
                    _PhonemeAtoms = new List<NoteAtomObject>() { new NoteAtomObject() };
                } 
                return _PhonemeAtoms;
            }
            set { _PhonemeAtoms = value; }
        }

        private double _VerbPrecent = 0.3;

        [DataMember]
        public double VerbPrecent
        {
            get { return _VerbPrecent; }
            set { if (value > 1)_VerbPrecent = 1; else if (value < 0)_VerbPrecent = 0; else  _VerbPrecent = value; }
        }


        public void InitNote()
        {
            try
            {
                _PhonemeAtoms.Clear();
                _PhonemeAtoms.Add(new NoteAtomObject());
                _PhonemeAtoms[0].InitNoteAtom();
            }
            catch { ;}
            Lyric = "a";
        }
        
        public object Clone()
        {
            return Force.DeepCloner.DeepClonerExtensions.DeepClone<NoteObject>(this);
        }
        public int CompareTo(Object o)
        {
            if (this.Tick > ((NoteObject)o).Tick)
                return 1;
            else if (this.Tick == ((NoteObject)o).Tick)
                return 0;
            else
                return -1;
        }
        public int Compare(NoteObject x, NoteObject y)
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
