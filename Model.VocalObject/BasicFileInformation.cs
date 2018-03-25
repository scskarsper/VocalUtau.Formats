using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;

namespace VocalUtau.Formats.Model.VocalObject
{
    [Serializable]
    [DataContract]
    public class BasicFileInformation
    {
        public static readonly string currentVersion = "0.1a";

        public BasicFileInformation()
        {
            _VersionString = currentVersion;
        }

        string _VersionString = currentVersion;
        [IgnoreDataMember]
        public string VersionString
        {
            get { return _VersionString; }
            set { _VersionString = value; }
        }

        string _IntroduceText = "";
        [IgnoreDataMember]
        public string IntroduceText
        {
            get { return _IntroduceText; }
            set { _IntroduceText = value; }
        }

        string _SavePassword = "";
        [DataMember]
        public string SavePassword
        {
            get { return _SavePassword; }
            set { _SavePassword = value; }
        }
    }
}
