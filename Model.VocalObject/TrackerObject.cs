using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class TrackerObject
    {
        SortedDictionary<double, PartsObject> _partList = new SortedDictionary<double, PartsObject>();
        [DataMember]
        public SortedDictionary<double, PartsObject> PartList
        {
            get { return _partList; }
            set { _partList = value; }
        }
    }
}
