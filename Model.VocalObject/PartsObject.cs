using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class PartsObject : IComparable, IComparer<PartsObject>, ICloneable,IPartsInterface
    {
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
        }
        public PartsObject(string PartName)
        {
            _PartName = PartName;
            _GUID = Guid.NewGuid().ToString();
        }

        string _SingerGUID = "";

        [DataMember]
        public string SingerGUID
        {
            get { return _SingerGUID; }
            set { _SingerGUID = value; }
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

        //long _TickLength = 0;

      //  [DataMember]
        public long TickLength
        {
            get { if (NoteList.Count == 0)return 480;
                
            //    return NoteList[NoteList.Count-1].Tick+NoteList[NoteList.Count-1].Length; 
            
                long max = 0;
                for (int i = 0; i < NoteList.Count; i++)
                {
                    max = Math.Max(max, NoteList[i].Tick + NoteList[i].Length);
                }
                return max;
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
