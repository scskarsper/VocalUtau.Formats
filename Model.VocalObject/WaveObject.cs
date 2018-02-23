using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [DataContract]
    public class WaveObject
    {
        public WaveObject()
        {
        }
        public WaveObject(string WavFileName)
        {
            this.wavfilename = WavFileName;
        }
        string wavfilename = "";
        [DataMember]
        public string WavFileName
        {
            get { return wavfilename; }
            set { wavfilename = value; }
        }

        double _DuringTime = 0;

        [DataMember]
        public double DuringTime
        {
            get { return _DuringTime; }
            set { _DuringTime = value; }
        }
    }
}
