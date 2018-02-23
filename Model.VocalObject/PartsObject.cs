using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class PartsObject
    {
        public PartsObject()
        {
        }
        public PartsObject(string PartName)
        {
            _PartName = PartName;
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

        double _Tempo = 120.0;

        [DataMember]
        public double Tempo
        {
            get { return _Tempo; }
            set { _Tempo = value; }
        }

        long _TickLength = 0;

        [DataMember]
        public long TickLength
        {
            get { return _TickLength; }
            set { _TickLength = value; }
        }
        
        [IgnoreDataMember]
        public double DuringTime
        {
            get { return Utils.MathUtils.Tick2Time(_TickLength, _Tempo); }
            set { _TickLength = Utils.MathUtils.Time2Tick(value, _Tempo); }
        }


        SortedDictionary<long, NoteObject> _NoteList = new SortedDictionary<long, NoteObject>();

        [DataMember]
        public SortedDictionary<long, NoteObject> NoteList
        {
            get { return _NoteList; }
            set { _NoteList = value; }
        }

        SortedDictionary<string, ControlPointObject<double>> _FullCurves = new SortedDictionary<string, ControlPointObject<double>>();

        [DataMember]
        public SortedDictionary<string, ControlPointObject<double>> FullCurves
        {
            get { return _FullCurves; }
            set { _FullCurves = value; }
        }

        ControlPointObject<double> _PitchBends = new ControlPointObject<double>();

        [DataMember]
        public ControlPointObject<double> PitchBends
        {
            get { return _PitchBends; }
            set { _PitchBends = value; }
        }

        public double getPitchBends(long Tick, double defaultValue)
        {
            return PitchBends.GetValue(Tick, defaultValue);
        }
    }
}
