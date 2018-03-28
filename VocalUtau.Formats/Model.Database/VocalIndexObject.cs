using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.USTs.Otos;
using VocalUtau.Formats.Model.Utils;
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

        SplitDictionary _SplitDictionary = new SplitDictionary();
        [DataMember]
        public SplitDictionary SplitDictionary
        {
            get { return _SplitDictionary; }
            set { _SplitDictionary = value; }
        }

        PrefixmapAtom _PrefixmapData = new PrefixmapAtom();
        [DataMember]
        public PrefixmapAtom PrefixAtomList
        {
            get { return _PrefixmapData; }
            set { _PrefixmapData = value; }
        }
        CharacterAtom _CharacertData = new CharacterAtom();
        [DataMember]
        public CharacterAtom CharacertData
        {
            get { return _CharacertData; }
            set { _CharacertData = value; }
        }
        List<string> _HashFiles = new List<string>();
        [DataMember]
        public List<string> HashFiles
        {
            get { return _HashFiles; }
            set { _HashFiles = value; }
        }
        string _HashValue = "";
        [DataMember]
        public string HashValue
        {
            get { return _HashValue; }
            set { _HashValue = value; }
        }

        private static string GetMD5HashFromFile(string fileName)
        {
            try
            {
                FileStream file = new FileStream(fileName, FileMode.Open);
                System.Security.Cryptography.MD5 md5 = new System.Security.Cryptography.MD5CryptoServiceProvider();
                byte[] retVal = md5.ComputeHash(file);
                file.Close();

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
            catch (Exception ex)
            {
                return "";
            }
        }
        private static string CalcHash(List<string> HashTable, string Folder)
        {
            string ret = "";
            for (int i = 0; i < HashTable.Count; i++)
            {
                ret += GetMD5HashFromFile(PathUtils.AbsolutePath(Folder,HashTable[i]));
            }
            return ret;
        }
        private static void SerializeTo(VocalIndexObject Object, string FilePath)
        {
            FileStream msObj = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream boz = new ICSharpCode.SharpZipLib.BZip2.BZip2OutputStream(msObj);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(VocalIndexObject));
            js.WriteObject(boz, Object);
            boz.Flush();
            boz.Close();
            msObj.Close();
        }
        private static VocalIndexObject SerializeFrom(string FilePath)
        {
            VocalIndexObject ret = null;
            try
            {
                FileStream msObj = new FileStream(FilePath, FileMode.Open, FileAccess.ReadWrite, FileShare.ReadWrite);
                ICSharpCode.SharpZipLib.BZip2.BZip2InputStream biz = new ICSharpCode.SharpZipLib.BZip2.BZip2InputStream(msObj);
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
            if (System.IO.File.Exists(Folder + "\\voicedbi.dat"))
            {
                ret = SerializeFrom(Folder + "\\voicedbi.dat");
                if (ret != null)
                {
                    string NewHash = CalcHash(ret.HashFiles, Folder);
                    if (NewHash != ret.HashValue)
                    {
                        ret = null;
                    }
                }
            }
            if (ret == null)
            {
                ret = new VocalIndexObject();
                ret.BasicData.IntroduceText = "Chorista Voice Index Cache File";
                ret.HashFiles.Add("character.txt");
                ret.CharacertData = CharacterSerializer.DeSerialize(Folder + "\\character.txt");
                ret.SndAtomList = OtoSerializer.DeSerialize(Folder, ret.HashFiles);
                ret.HashFiles.Add("prefix.map");
                if (System.IO.File.Exists(Folder + "\\prefix.map"))
                {
                    ret.PrefixAtomList = PrefixMapSerialzier.DeSerialize(Folder + "\\prefix.map");
                }
                SplitDictionary sdlib = null;
                if (System.IO.File.Exists(Folder + "\\splitdic.json"))
                {
                    sdlib = SplitDictionary.SerializeFrom(Folder + "\\splitdic.json");
                    if (ret != null)
                    {
                        ret.HashFiles.Add("splitdic.json");
                    }
                }
                if (sdlib == null && System.IO.File.Exists(Folder + "\\presamp.ini"))
                {
                    sdlib = Presamp2DictSerializer.DeSerialize(Folder + "\\presamp.ini");
                    if (sdlib != null)
                    {
                        ret.HashFiles.Add("presamp.ini");
                    }
                }
                if (sdlib != null)
                {
                    ret.SplitDictionary = sdlib;
                }
                else
                {
                    ret.SplitDictionary = new SplitDictionary();
                }
                ret.SplitDictionary.MapSndList(ret.SndAtomList, ret.PrefixAtomList);
                ret.HashValue = CalcHash(ret.HashFiles, Folder);
                SerializeTo(ret, Folder + "\\voicedbi.dat");
            }
            return ret;
        }
    }
}
