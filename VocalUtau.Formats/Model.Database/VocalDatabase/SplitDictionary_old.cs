using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading.Tasks;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    [Serializable]
    [DataContract]
    public class SplitDictionary_old
    {
        [Serializable]
        [DataContract]
        public class SplitAtom
        {
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
        }

        [Serializable]
        [DataContract]
        public class SplitMap
        {
            List<SplitAtom> _preAtom = null;
            [DataMember]
            public List<SplitAtom> PreAtom
            {
                get { return _preAtom; }
                set { _preAtom = value; }
            }
            List<SplitAtom> _nxtAtom = null;
            [DataMember]
            public List<SplitAtom> NxtAtom
            {
                get { return _nxtAtom; }
                set { _nxtAtom = value; }
            }
        }

        Dictionary<string, SplitMap> _PhonemeMap = new Dictionary<string, SplitMap>();//TEMP
        [DataMember]
        public Dictionary<string, SplitMap> PhonemeMap
        {
          get { return _PhonemeMap; }
          set { _PhonemeMap = value; }
        }

        public static void SerializeTo(SplitDictionary_old Object, string FilePath)
        {
            FileStream msObj = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream boz = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msObj);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(VocalIndexObject));
            js.WriteObject(boz, Object);
            boz.Flush();
            msObj.Close();
        }
        public static SplitDictionary_old SerializeFrom(string FilePath)
        {
            SplitDictionary_old ret = null;
            try
            {
                FileStream msObj = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                ICSharpCode.SharpZipLib.GZip.GZipInputStream biz = new ICSharpCode.SharpZipLib.GZip.GZipInputStream(msObj);
                string toDes = "";
                using (StreamReader sr = new StreamReader(biz))
                {
                    try
                    {
                        toDes = sr.ReadToEnd();
                    }
                    catch { ; }
                }
                using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(toDes)))
                {
                    try
                    {
                        DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(SplitDictionary_old));
                        SplitDictionary_old model = (SplitDictionary_old)(deseralizer.ReadObject(ms));
                        if (model != null)
                        {
                            ret = model;
                        }
                    }
                    catch { ;}
                }
                msObj.Close();
            }
            catch { ;}
            return ret;
        }
    }
}
