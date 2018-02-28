using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class NoteAtomObject
    {
        public NoteAtomObject()
        {
        }
        public NoteAtomObject(string Phoneme)
        {
            _PhonemeAtom = Phoneme;
        }

        public void InitNoteAtom()
        {
        }

        double _AtomLength = 0;

        public double AtomLength
        {
            get { return _AtomLength; }
            set { _AtomLength = value; }
        }

        string _PhonemeAtom = "";

        [DataMember]
        public string PhonemeAtom
        {
            get { return _PhonemeAtom; }
            set { _PhonemeAtom = value; }
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


        double _PreUtterance;

        [DataMember]
        public double PreUtterance
        {
            get { return _PreUtterance; }
            set { _PreUtterance = value; }
        }
        double _Overlap;

        [DataMember]
        public double Overlap
        {
            get { return _Overlap; }
            set { _Overlap = value; }
        }
        double _Intensity;

        [DataMember]
        public double Intensity
        {
            get { return _Intensity; }
            set { _Intensity = value; }
        }
        double _Modulation;

        [DataMember]
        public double Modulation
        {
            get { return _Modulation; }
            set { _Modulation = value; }
        }

        double _StartPoint;

        [DataMember]
        public double StartPoint
        {
            get { return _StartPoint; }
            set { _StartPoint = value; }
        }
        double _Velocity;

        [DataMember]
        public double Velocity
        {
            get { return _Velocity; }
            set { _Velocity = value; }
        }

        List<KeyValuePair<double, double>> _Envelopes = new List<KeyValuePair<double, double>>();

        [DataMember]
        public List<KeyValuePair<double, double>> Envelopes
        {
            get { return _Envelopes; }
            set { _Envelopes = value; }
        }
    }
}
