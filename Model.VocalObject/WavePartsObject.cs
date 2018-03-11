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
    public class WavePartsObject : IComparable, IComparer<WavePartsObject>, ICloneable, IPartsInterface
    {
        string _GUID = "";

        public string GUID
        {
            get { return _GUID; }
            set { _GUID = value; }
        }
        public string getGuid()
        {
            return GUID;
        }

        public WavePartsObject()
        {
            _GUID = Guid.NewGuid().ToString();
        }
        public WavePartsObject(string WavFileName)
        {
            this.wavfilename = WavFileName;
            _GUID = Guid.NewGuid().ToString();
        }
        string wavfilename = "";
        [DataMember]
        public string WavFileName
        {
            get { return wavfilename; }
            set { wavfilename = value; }
        }

        string _PartName = "";

        [DataMember]
        public string PartName
        {
            get { return _PartName; }
            set { _PartName = value; }
        }


        double _DuringTime = 0;

        [DataMember]
        public double DuringTime
        {
            get { return _DuringTime; }
            set { _DuringTime = value; }
        }


        double _StartTime = 0;

        [DataMember]
        public double StartTime
        {
            get { return _StartTime; }
            set { _StartTime = value; }
        }

        public long getAbsoluteStartTick(double Tempo = -1)
        {
            if (Tempo < 0) Tempo = 120.0;
            return Utils.MidiMathUtils.Time2Tick(_StartTime, Tempo);
        }
        public void setAbsoluteStartTick(long value, double Tempo = -1)
        {
            if (Tempo < 0) Tempo = 120.0;
            _StartTime = Utils.MidiMathUtils.Tick2Time(value, Tempo);
        }
        public long getAbsoluteEndTick(double Tempo = -1)
        {
            if (Tempo < 0) Tempo = 120.0;
            return Utils.MidiMathUtils.Time2Tick(_StartTime + DuringTime, Tempo);
        }


        public double getDuringTime()
        {
            return DuringTime;
        }
        public void setDuringTime(double DuringTime)
        {
            this.DuringTime = DuringTime;
        }
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
            if (this.StartTime > ((WavePartsObject)o).StartTime)
                return 1;
            else if (this.StartTime == ((WavePartsObject)o).StartTime)
                return 0;
            else
                return -1;
        }
        public int Compare(WavePartsObject x, WavePartsObject y)
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
