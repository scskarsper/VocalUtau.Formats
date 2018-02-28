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
        List<PartsObject> _partList = new List<PartsObject>();
        [DataMember]
        public List<PartsObject> PartList
        {
            get { return _partList; }
            set { _partList = value; }
        }
    }
}
