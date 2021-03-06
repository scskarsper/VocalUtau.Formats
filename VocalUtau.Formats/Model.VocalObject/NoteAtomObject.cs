﻿using System;
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

        long _AtomLength = 0;
        [DataMember]
        public long AtomLength
        {
            get { return _AtomLength; }
            set { _AtomLength = value; }
        }

        bool _LengthIsPercent = false;
        [DataMember]
        public bool LengthIsPercent
        {
            get { return _LengthIsPercent; }
            set { _LengthIsPercent = value; }
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


        double _PreUtterance = double.NaN;

        [DataMember]
        public double PreUtterance
        {
            get { return _PreUtterance; }
            set { _PreUtterance = value; }
        }
        double _Overlap = double.NaN;

        [DataMember]
        public double Overlap
        {
            get { return _Overlap; }
            set { _Overlap = value; }
        }
        double _Intensity = double.NaN;

        [DataMember]
        public double Intensity
        {
            get { return _Intensity; }
            set { _Intensity = value; }
        }
        double _Modulation = double.NaN;

        [DataMember]
        public double Modulation
        {
            get { return _Modulation; }
            set { _Modulation = value; }
        }

        double _StartPoint = double.NaN;

        [DataMember]
        public double StartPoint
        {
            get { return _StartPoint; }
            set { _StartPoint = value; }
        }
        double _Velocity = double.NaN;

        [DataMember]
        public double Velocity
        {
            get { return _Velocity; }
            set { 
            if (double.IsNaN(value))
            {
                _Velocity = value;
            }
            else if (double.IsInfinity(value))
            {
                _Velocity = double.NaN;
            }
            else if (value > 1000)
            {
                _Velocity = 1000;
            }
            else if (value < 0)
            {
                _Velocity = 0;
            }
            else
            {
                _Velocity = (int)value;
            }
            }
        }

        //List<KeyValuePair<double, double>> _Envelopes = new List<KeyValuePair<double, double>>();

        //public List<KeyValuePair<double, double>> Envelopes
        //{
        //    get { return _Envelopes; }
        //    set { _Envelopes = value; }
        //}


        long _fadeInLengthMs = 5;

        [DataMember]
        public long FadeInLengthMs
        {
            get { return _fadeInLengthMs; }
            set { _fadeInLengthMs = value; }
        }

        long _fadeOutLengthMs = 35;

        [DataMember]
        public long FadeOutLengthMs
        {
            get { return _fadeOutLengthMs; }
            set { _fadeOutLengthMs = value; }
        }

        long _volumePercentInt = 100;

        [DataMember]
        public long VolumePercentInt
        {
            get { return _volumePercentInt; }
            set { _volumePercentInt = value; }
        }
    }
}
