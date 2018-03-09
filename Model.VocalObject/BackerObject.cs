using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class BackerObject
    {
        public BackerObject(uint index)
        {
            this.Index = index;
        }
        SortedDictionary<double, WaveObject> _wavPartList = new SortedDictionary<double, WaveObject>();
        [DataMember]
        public SortedDictionary<double, WaveObject> WavPartList
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
                    return "Background Music " + Index.ToString();
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
    }
}
