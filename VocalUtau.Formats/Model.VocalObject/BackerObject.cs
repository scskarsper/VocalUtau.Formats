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
    public class BackerObject : IComparable,  ICloneable, ITrackerInterface, IComparer<BackerObject>
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

        public BackerObject(uint index)
        {
            this.Index = index;
        }
        List<WavePartsObject> _wavPartList = new List<WavePartsObject>();
        [DataMember]
        public List<WavePartsObject> WavPartList
        {
            get { return _wavPartList; }
            set { _wavPartList = value; }
        }
        string _name = "";
        [DataMember]
        public string Name
        {
            get
            {
                if (_name == "")
                {
                    _name = "Background Music " + Index.ToString();
                    return _name;
                }
                else
                {
                    return _name;
                }
            }
            set { _name = value; }
        }

        private uint _index;
        [DataMember]
        public uint Index
        {
            get { return _index; }
            set { _index = value; }
        }


        public uint getIndex()
        {
            return _index;
        }
        public void setIndex(uint Index)
        {
            _index = Index;
        }
        public string getName()
        {
            return Name;
        }
        public void setName(string Name)
        {
            this.Name = Name;
        }

        private double _volume = 0.8;
        [DataMember]
        public double Volume
        {
            get { return _volume; }
            set { _volume = value; if (_volume < 0)_volume = 0; if (_volume > 2)_volume = 2; }
        }
        public double getVolume()
        {
            return this.Volume;
        }
        public void setVolume(double volume)
        {
            this.Volume = volume;
        }

        public object Clone()
        {
            return Force.DeepCloner.DeepClonerExtensions.DeepClone<BackerObject>(this);
        }
        public int CompareTo(Object o)
        {
            if (this.Index > ((BackerObject)o).Index)
                return 1;
            else if (this.Index == ((BackerObject)o).Index)
                return 0;
            else
                return -1;
        }
        public int Compare(BackerObject x, BackerObject y)
        {
            if (x.Index < y.Index)
                return -1;
            else if (x.Index == y.Index)
                return 0;
            else
                return 1;
        }


        [IgnoreDataMember]
        public double TotalLength
        {
            get
            {
                if (_wavPartList.Count == 0) return 0;
                return _wavPartList[_wavPartList.Count - 1].StartTime + _wavPartList[_wavPartList.Count - 1].DuringTime;
            }
        }

        public void OrderList()
        {
            double HeadPtr = double.MinValue;
            _wavPartList.Sort();
            for (int i = 0; i < _wavPartList.Count; i++)
            {
                if (HeadPtr > _wavPartList[i].StartTime)
                {
                    _wavPartList[i].StartTime = HeadPtr;
                }
                HeadPtr = _wavPartList[i].StartTime + _wavPartList[i].DuringTime;
            }
        }
        public bool CheckOrdered()
        {
            bool ret = true;
            double HeadPtr = double.MinValue;
            _wavPartList.Sort();
            for (int i = 0; i < _wavPartList.Count; i++)
            {
                if (HeadPtr > _wavPartList[i].StartTime)
                {
                    ret = false;
                    break;
                }
                HeadPtr = _wavPartList[i].StartTime + _wavPartList[i].DuringTime;
            }
            return ret;
        }
    }
}
