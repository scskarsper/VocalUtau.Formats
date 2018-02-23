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
        SortedDictionary<double, WaveObject> _wavPartList = new SortedDictionary<double, WaveObject>();
        [DataMember]
        public SortedDictionary<double, WaveObject> WavPartList
        {
            get { return _wavPartList; }
            set { _wavPartList = value; }
        }
    }
}
