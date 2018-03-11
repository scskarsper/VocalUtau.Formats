using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class TrackerObject : ITrackerInterface
    {
        public TrackerObject(uint index)
        {
            this.Index = index;
        }
        List<PartsObject> _partList = new List<PartsObject>();
        [DataMember]
        public List<PartsObject> PartList
        {
            get { return _partList; }
            set { _partList = value; }
        }

        [IgnoreDataMember]
        public double TotalLength
        {
            get
            {
                if (_partList.Count == 0) return 0;
                return _partList[_partList.Count - 1].StartTime+_partList[_partList.Count - 1].DuringTime; ;
            }
        }

        public void OrderList()
        {
            double HeadPtr = double.MinValue;
            _partList.Sort();
            for (int i = 0; i < _partList.Count; i++)
            {
                if (HeadPtr > _partList[i].StartTime)
                {
                    _partList[i].StartTime = HeadPtr;
                }
                HeadPtr = _partList[i].StartTime + _partList[i].DuringTime;
            }
        }
        public bool CheckOrdered()
        {
            bool ret = true;
            double HeadPtr = double.MinValue;
            _partList.Sort();
            for (int i = 0; i < _partList.Count; i++)
            {
                if (HeadPtr > _partList[i].StartTime)
                {
                    ret = false;
                    break;
                }
                HeadPtr = _partList[i].StartTime + _partList[i].DuringTime;
            }
            return ret;
        }

        string _name = "";
        [DataMember]
        public string Name
        {
            get
            {
                if (_name == "")
                {
                    return "Vocal Track "+Index.ToString();
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


        public int CompareTo(Object o)
        {
            if (this.Index > ((TrackerObject)o).Index)
                return 1;
            else if (this.Index == ((TrackerObject)o).Index)
                return 0;
            else
                return -1;
        }
        public int Compare(TrackerObject x, TrackerObject y)
        {
            if (x.Index < y.Index)
                return -1;
            else if (x.Index == y.Index)
                return 0;
            else
                return 1;
        }
    }
}
