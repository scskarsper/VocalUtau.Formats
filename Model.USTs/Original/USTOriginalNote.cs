using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace VocalUtau.Formats.Model.USTs.Original
{
    public class USTOriginalNote
    {
        long _Length;

        public long Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        string _Lyric;

        public string Lyric
        {
            get { return _Lyric; }
            set { _Lyric = value; }
        }

        int _NoteNum;

        public int NoteNum
        {
            get { return _NoteNum; }
            set { _NoteNum = value; }
        }

        double _PreUtterance;

        public double PreUtterance
        {
            get { return _PreUtterance; }
            set { _PreUtterance = value; }
        }
        double _Overlap;

        public double Overlap
        {
            get { return _Overlap; }
            set { _Overlap = value; }
        }
        double _Intensity;

        public double Intensity
        {
            get { return _Intensity; }
            set { _Intensity = value; }
        }
        double _Modulation;

        public double Modulation
        {
            get { return _Modulation; }
            set { _Modulation = value; }
        }
        double _Tempo;

        public double Tempo
        {
            get { return _Tempo; }
            set { _Tempo = value; }
        }
        double _StartPoint;

        public double StartPoint
        {
            get { return _StartPoint; }
            set { _StartPoint = value; }
        }
        double _Velocity;

        public double Velocity
        {
            get { return _Velocity; }
            set { _Velocity = value; }
        }
        string _Flags;

        public string Flags
        {
            get { return _Flags; }
            set { _Flags = value; }
        }
        string _Envelope;

        public string Envelope
        {
            get { return _Envelope; }
            set { _Envelope = value; }
        }

        public List<KeyValuePair<double, double>> EnvelopAnalyse()
        {
            List<KeyValuePair<double, double>> Ret = new List<KeyValuePair<double, double>>();
            double p1 = 0, p2 = 5, p3 = 5, p4 = 0;
            double v1 = 0, v2 = 100, v3 = 100, v4 = 0;
            string[] EAr = _Envelope.Split(',');
            if (EAr.Length > 6)
            {
                double.TryParse(EAr[0], out p1);
                double.TryParse(EAr[1], out p2);
                double.TryParse(EAr[2], out p3);
                double.TryParse(EAr[3], out v1);
                double.TryParse(EAr[4], out v2);
                double.TryParse(EAr[5], out v3);
                double.TryParse(EAr[6], out v4);
                Ret.Add(new KeyValuePair<double, double>(p1, v1));
                Ret.Add(new KeyValuePair<double, double>(p2, v2));
                Ret.Add(new KeyValuePair<double, double>(p3, v3));
            }
            if (EAr.Length > 7 && EAr[7].Trim() == "%")
            {
                double.TryParse(EAr[8], out p4);
                Ret.Add(new KeyValuePair<double, double>(p4, v4));
            }
            else if (EAr.Length > 6)
            { 
                Ret.Add(new KeyValuePair<double, double>(p4, v4)); 
            }

            if (EAr.Length > 10 && EAr[7].Trim() == "%")
            {
                double p5 = 0; double v5 = v2;
                double.TryParse(EAr[9], out p5);
                double.TryParse(EAr[10], out v5);
                Ret.Add(new KeyValuePair<double, double>(p5, v5)); 
            }
            return Ret;
        }
        double _Apreuttr;

        public double Apreuttr
        {
            get { return _Apreuttr; }
            set { _Apreuttr = value; }
        }
        double _Aoverlap;

        public double Aoverlap
        {
            get { return _Aoverlap; }
            set { _Aoverlap = value; }
        }
        double _Astpoint;

        public double Astpoint
        {
            get { return _Astpoint; }
            set { _Astpoint = value; }
        }
        int _PBType;

        public int PBType
        {
            get { return _PBType; }
            set { _PBType = value; }
        }
        string _PitchBend;

        public string PitchBend
        {
            get { return _PitchBend; }
            set { _PitchBend = value; }
        }
        int _PBStart;

        public int PBStart
        {
            get { return _PBStart; }
            set { _PBStart = value; }
        }
        string _VBR;

        public string VBR
        {
            get { return _VBR; }
            set { _VBR = value; }
        }
        string _PBW;

        public string PBW
        {
            get { return _PBW; }
            set { _PBW = value; }
        }
        string _PBY;

        public string PBY
        {
            get { return _PBY; }
            set { _PBY = value; }
        }
        string _PBS;

        public string PBS
        {
            get { return _PBS; }
            set { _PBS = value; }
        }
    }
}
