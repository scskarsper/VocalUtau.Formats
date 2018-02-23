using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class NoteObject
    {
        long _TickLength = 0;

        [DataMember]
        public long TickLength
        {
            get { return _TickLength; }
            set { _TickLength = value; }
        }

        string _Lyric = "";

        [DataMember]
        public string Lyric
        {
            get { return _Lyric; }
            set { 
                _Lyric = value;
                if (_PhonemeAtoms.Count == 1)
                {
                    if (_PhonemeAtoms[0].PhonemeAtom == "")
                    {
                        _PhonemeAtoms[0].PhonemeAtom = _Lyric;
                    }
                }
            }
        }
        
        int _NoteNum;

        [DataMember]
        public int NoteNum
        {
            get { return _NoteNum; }
            set { _NoteNum = value; }
        }

        List<NoteAtomObject> _PhonemeAtoms = new List<NoteAtomObject>() { new NoteAtomObject() };

        [DataMember]
        public List<NoteAtomObject> PhonemeAtoms
        {
            get { return _PhonemeAtoms; }
            set { _PhonemeAtoms = value; }
        }
    }
}
