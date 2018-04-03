using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using VocalUtau.Formats.Model.VocalObject.ParamTranslater;
using VocalUtau.Formats.Model.BaseObject;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class PartsObject : IComparable, IComparer<PartsObject>, ICloneable,IPartsInterface
    {
        [NonSerialized]
        PitchCompiler _PitchCompiler;
        [IgnoreDataMember]
        public PitchCompiler PitchCompiler
        {
            get
            {
                if (_PitchCompiler == null)
                {
                    PartsObject po = this; _PitchCompiler = new PitchCompiler(ref po);
                }; 
                return _PitchCompiler;
            }
        }

        [NonSerialized]
        DynCompiler _DynCompiler;
        [IgnoreDataMember]
        public DynCompiler DynCompiler
        {
            get
            {
                if (_DynCompiler == null)
                {
                    PartsObject po = this; _DynCompiler = new DynCompiler(ref po);
                };
                return _DynCompiler;
            }
        }

        [NonSerialized]
        NoteCompiler _NoteCompiler;
        [IgnoreDataMember]

        public NoteCompiler NoteCompiler
        {
            get { if (_NoteCompiler == null) { PartsObject po = this; _NoteCompiler = new NoteCompiler(ref po); }; return _NoteCompiler; }
            set { _NoteCompiler = value; }
        }

        string _GUID = "";

        [DataMember]
        public string GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        public string getGuid()
        {
            return GUID;
        }


        public PartsObject()
        {
            _GUID = Guid.NewGuid().ToString();
            PartsObject po = this;
            _PitchCompiler = new PitchCompiler(ref po);
            _DynCompiler = new DynCompiler(ref po);
            _NoteCompiler = new NoteCompiler(ref po);
        }
        public PartsObject(string PartName)
        {
            _PartName = PartName;
            _GUID = Guid.NewGuid().ToString();
            PartsObject po = this;
            _PitchCompiler = new PitchCompiler(ref po);
            _DynCompiler = new DynCompiler(ref po);
            _NoteCompiler = new NoteCompiler(ref po);
        }

        string _SingerGUID = "";

        [DataMember]
        public string SingerGUID
        {
            get { return _SingerGUID; }
            set { _SingerGUID = value; }
        }

        bool _LyricDicitonary = true;

        [DataMember]
        public bool UseLyricDicitonary
        {
            get { return _LyricDicitonary; }
            set { _LyricDicitonary = value; }
        }

        string _PartName = "";

        [DataMember]
        public string PartName
        {
            get { return _PartName; }
            set { _PartName = value; }
        }

        string _PartResampler = "";

        public string PartResampler
        {
            get { return _PartResampler; }
            set { _PartResampler = value; }
        }
        public string getResampler(string Default)
        {
            if (_PartResampler != "") return _PartResampler;
            return Default;
        }

        string _Flags = "";

        [DataMember]
        public string Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }
        public string getFlags(string defaultFlags)
        {
            if (_Flags == "")
            {
                return defaultFlags;
            }
            else
            {
                return _Flags;
            }
        }

        double _BaseTempo = 120.0;

        [DataMember]
        public double BaseTempo
        {
            get { return _BaseTempo; }
            set { _BaseTempo = value; }
        }

        double _Tempo = double.NaN;

        public double getRealTempo()
        {
            return _Tempo;
        }

        [DataMember]
        public double Tempo
        {
            get { return double.IsNaN(_Tempo) ? _BaseTempo : _Tempo; }
            set { _Tempo = value; }
        }

        public long TickLength
        {
            get { if (NoteList.Count == 0)return 480;
                
                return NoteList[NoteList.Count-1].Tick+NoteList[NoteList.Count-1].Length; 
            }
        }

        double _StartTime = 0;

        [DataMember]
        public double StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        public long getAbsoluteStartTick(double Tempo=-1)
        {
            if (Tempo < 0) Tempo = this.Tempo;
            return Utils.MidiMathUtils.Time2Tick(_StartTime, Tempo);
        }
        public void setAbsoluteStartTick(long value,double Tempo = -1)
        {
            if (Tempo < 0) Tempo = this.Tempo;
            _StartTime = Utils.MidiMathUtils.Tick2Time(value, Tempo);
        }
        public long getAbsoluteEndTick(double Tempo = -1)
        {
            if (Tempo < 0) Tempo = this.Tempo;
            return Utils.MidiMathUtils.Time2Tick(_StartTime+DuringTime, Tempo);
        }
        
        [IgnoreDataMember]
        public double DuringTime
        {
            get { return Utils.MidiMathUtils.Tick2Time(TickLength, Tempo); }
          //  set { _TickLength = Utils.MidiMathUtils.Time2Tick(value, Tempo); }
        }

        public double getDuringTime()
        {
            return DuringTime;
        }
       /* public void setDuringTime(double DuringTime)
        {
            this.DuringTime = DuringTime;
        }*/
        public string getPartName()
        {
            return PartName;
        }
        public void setPartName(string Name)
        {
            this.PartName = Name;
        }
        public double getStartTime()
        {
            return StartTime;
        }
        public void setStartTime(double StartTime)
        {
            this.StartTime = StartTime;
        }

        List<NoteObject> _NoteList = new List<NoteObject>();
        TickSortList<PitchObject> _PitchList = new TickSortList<PitchObject>();
        private TickSortList<ControlObject> _DynList = new TickSortList<ControlObject>();
        TickSortList<PitchObject> _BasePitchList = new TickSortList<PitchObject>();

        [IgnoreDataMember]
        public TickSortList<PitchObject> BasePitchList
        {
            get { return _BasePitchList; }
            set { _BasePitchList = value; }
        }

        [DataMember]
        public List<NoteObject> NoteList
        {
            get { return _NoteList; }
            set { _NoteList = value; }
        }

        [DataMember]
        public TickSortList<ControlObject> DynList
        {
            get { return _DynList; }
            set { _DynList = value; }
        }

        [DataMember]
        public TickSortList<PitchObject> PitchList
        {
            get { return _PitchList; }
            set { _PitchList = value; }
        }
        
        private int _DynBaseValue = 100;

        [DataMember]
        public int DynBaseValue
        {
            get { return _DynBaseValue; }
            set { _DynBaseValue = value; }
        }
        
        public object Clone()
        {
            return Force.DeepCloner.DeepClonerExtensions.DeepClone<PartsObject>(this);
        }
        public int CompareTo(Object o)
        {
            if (this.StartTime > ((PartsObject)o).StartTime)
                return 1;
            else if (this.StartTime == ((PartsObject)o).StartTime)
                return 0;
            else
                return -1;
        }
        public int Compare(PartsObject x, PartsObject y)
        {
            if (x.StartTime < y.StartTime)
                return -1;
            else if (x.StartTime == y.StartTime)
                return 0;
            else
                return 1;
        }
    }
}
