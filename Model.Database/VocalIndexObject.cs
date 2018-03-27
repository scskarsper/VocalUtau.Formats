using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.USTs.Otos;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.Database
{
    [Serializable]
    [DataContract]
    public class VocalIndexObject
    {
        private BasicFileInformation _BasicData = new BasicFileInformation();
        [DataMember]
        public BasicFileInformation BasicData
        {
            get { return _BasicData; }
            set { _BasicData = value; }
        }

        List<SoundAtom> _SndAtomList = new List<SoundAtom>();
        [DataMember]
        public List<SoundAtom> SndAtomList
        {
            get { return _SndAtomList; }
            set { _SndAtomList = value; }
        }

        List<PrefixAtom> _PrefixAtomList = new List<PrefixAtom>();
        [DataMember]
        public List<PrefixAtom> PrefixAtomList
        {
            get { return _PrefixAtomList; }
            set { _PrefixAtomList = value; }
        }



        private static void SerializeTo(VocalIndexObject Object, string FilePath)
        {
            FileStream msObj = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(VocalIndexObject));
            js.WriteObject(msObj, Object);
            msObj.Close();
        }
        private static VocalIndexObject SerializeFrom(string FilePath)
        {
            VocalIndexObject ret = null;
            try
            {
                FileStream msObj = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                string toDes = "";
                using (StreamReader sr = new StreamReader(msObj))
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
                        DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(VocalIndexObject));
                        VocalIndexObject model = (VocalIndexObject)(deseralizer.ReadObject(ms));
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
        public static VocalIndexObject Deseralize(string Folder)
        {
            VocalIndexObject ret = null;
            if (System.IO.File.Exists(Folder + "\\voiceIndex.dat"))
            {
                ret = SerializeFrom(Folder + "\\voiceIndex.dat");
            }
            if (ret == null)
            {
                ret = new VocalIndexObject();
                ret.BasicData.IntroduceText="Chorista Voice Index Cache File";
                ret.SndAtomList = OtoSerializer.DeSerialize(Folder);
            }
            return ret;
        }
    }
}
