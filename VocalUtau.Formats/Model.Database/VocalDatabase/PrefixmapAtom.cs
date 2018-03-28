using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    [Serializable]
    [DataContract]
    public class PrefixmapAtom
    {
        //前缀
        Dictionary<uint, string> _PreFix = new Dictionary<uint, string>();

        [DataMember]
        public Dictionary<uint, string> PreFix
        {
            get { return _PreFix; }
            set { _PreFix = value; }
        }
        //后缀
        Dictionary<uint, string> _SufFix = new Dictionary<uint, string>() ;

        [DataMember]
        public Dictionary<uint, string> SufFix
        {
            get { return _SufFix; }
            set { _SufFix = value; }
        }

        List<string> _prefixList = new List<string>() { "" };
        [DataMember]
        public List<string> PrefixList
        {
            get { return _prefixList; }
            set { _prefixList = value; }
        }

        List<string> _suffixList = new List<string>() { "" };
        [DataMember]
        public List<string> SuffixList
        {
            get { return _suffixList; }
            set { _suffixList = value; }
        }

        public PrefixmapAtom()
        {
            for (int i = 0; i < 120; i++)
            {
                _PreFix.Add((uint)i, "");
                _SufFix.Add((uint)i, "");
            } 
        }
    }
}
