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
    public class PartsObject : IComparable, IComparer<PartsObject>, ICloneable
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

        double _StartTime = 0;

        [DataMember]
        public double StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        [IgnoreDataMember]
        public long AbsoluteStartTick
        {
            get { return Utils.MidiMathUtils.Time2Tick(_StartTime, _Tempo); }
            set { _StartTime = Utils.MidiMathUtils.Tick2Time(value, _Tempo); }
        }
        
        [IgnoreDataMember]
        public double DuringTime
        {
            get { return Utils.MidiMathUtils.Tick2Time(_TickLength, _Tempo); }
            set { _TickLength = Utils.MidiMathUtils.Time2Tick(value, _Tempo); }
        }


        List<NoteObject> _NoteList = new List<NoteObject>();
        List<PitchObject> _PitchList = new List<PitchObject>();

        [DataMember]
        public List<NoteObject> NoteList
        {
            get { return _NoteList; }
            set { _NoteList = value; }
        }

        SortedDictionary<string, List<ControlObject>> _FullCurves = new SortedDictionary<string, List<ControlObject>>();

        [DataMember]
        public SortedDictionary<string, List<ControlObject>> FullCurves
        {
            get { return _FullCurves; }
            set { _FullCurves = value; }
        }

        private int _DynBaseValue = 100;

        [DataMember]
        public int DynBaseValue
        {
            get { return _DynBaseValue; }
            set { _DynBaseValue = value; }
        }

        private List<ControlObject> _DynList = new List<ControlObject>();
        [DataMember]
        public List<ControlObject> DynList
        {
            get { return _DynList; }
            set { _DynList = value; }
        }

        [DataMember]
        public List<PitchObject> PitchBendsList
        {
            get { return _PitchList; }
            set { _PitchList = value; }
        }

        public void OrderList()
        {
            long HeadPtr = long.MinValue;
            _NoteList.Sort();
            for (int i = 0; i < _NoteList.Count; i++)
            {
                if (HeadPtr > _NoteList[i].Tick)
                {
                    long NoteEnd = _NoteList[i].Tick + _NoteList[i].Length;
                    if (NoteEnd > HeadPtr)
                    {
                        //后面有多余出来的
                        _NoteList[i].Length = NoteEnd - HeadPtr;
                        _NoteList[i].Tick = HeadPtr;
                        if (_NoteList[i].Length < 32)
                        {
                            //小于32tick，无效音符
                            _NoteList.RemoveAt(i);
                            i--;
                        }
                        else
                        {
                            //切断尾长
                            HeadPtr = _NoteList[i].Tick + _NoteList[i].Length;
                        }
                    }
                    else
                    {
                        _NoteList.RemoveAt(i);
                        i--;
                    }
                }
                else
                {
                    HeadPtr = _NoteList[i].Tick + _NoteList[i].Length;
                }
            }
        }
        public bool CheckOrdered()
        {
            bool ret = true;
            long HeadPtr = long.MinValue;
            _PitchList.Sort();
            _NoteList.Sort();
            for (int i = 0; i < _NoteList.Count; i++)
            {
                if (HeadPtr > _NoteList[i].Tick)
                {
                    ret = false;
                    break;
                }
                HeadPtr = _NoteList[i].Tick + _NoteList[i].Length;
            }
            return ret;
        }

        public object Clone()
        {
            BinaryFormatter Formatter = new BinaryFormatter(null, new StreamingContext(StreamingContextStates.Clone));
            MemoryStream stream = new MemoryStream();
            Formatter.Serialize(stream, this);
            stream.Position = 0;
            object clonedObj = Formatter.Deserialize(stream);
            stream.Close();
            return clonedObj;
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
