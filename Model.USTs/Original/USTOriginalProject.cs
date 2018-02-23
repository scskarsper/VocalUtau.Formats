using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.USTs.Original
{
    public class USTOriginalProject
    {
        double _Tempo;

        public double Tempo
        {
            get { return _Tempo; }
            set { _Tempo = value; }
        }

        int _Tracks;

        public int Tracks
        {
            get { return _Tracks; }
            set { _Tracks = value; }
        }

        string _ProjectName;

        public string ProjectName
        {
            get { return _ProjectName; }
            set { _ProjectName = value; }
        }

        string _VoiceDir;

        public string VoiceDir
        {
            get { return _VoiceDir; }
            set { _VoiceDir = value; }
        }

        string _OutFile;

        public string OutFile
        {
            get { return _OutFile; }
            set { _OutFile = value; }
        }

        string _CacheDir;

        public string CacheDir
        {
            get { return _CacheDir; }
            set { _CacheDir = value; }
        }

        string _Tool1;

        public string Tool1
        {
            get { return _Tool1; }
            set { _Tool1 = value; }
        }

        string _Tool2;

        public string Tool2
        {
            get { return _Tool2; }
            set { _Tool2 = value; }
        }

        bool _Mode2;

        public bool Mode2
        {
            get { return _Mode2; }
            set { _Mode2 = value; }
        }

        string _Flags;

        public string Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }

        SortedDictionary<long, USTOriginalNote> _Notes = new SortedDictionary<long, USTOriginalNote>();

        public SortedDictionary<long, USTOriginalNote> Notes
        {
            get { return _Notes; }
            set { _Notes = value; }
        }
    }
}
