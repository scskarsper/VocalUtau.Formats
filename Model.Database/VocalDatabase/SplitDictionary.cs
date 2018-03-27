using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    [Serializable]
    [DataContract]
    public class SplitDictionary
    {
        Dictionary<string, string> _PhonemeMap = new Dictionary<string, string>();//TEMP
        [DataMember]
        public Dictionary<string, string> PhonemeMap
        {
          get { return _PhonemeMap; }
          set { _PhonemeMap = value; }
        }

        public void MapSndList(List<SoundAtom> SndList, PrefixmapAtom Prefixmap)
        {
            foreach(SoundAtom snd in SndList)
            {
                string LyricSnd = snd.PhonemeSymbol;
                if (Prefixmap != null)
                {
                    for (int i = 0; i < Prefixmap.SuffixList.Count; i++)
                    {
                        if (Prefixmap.SuffixList[i] != "")
                        {
                            int lastidx = LyricSnd.LastIndexOf(Prefixmap.SuffixList[i]);
                            if (lastidx > 0 && lastidx == LyricSnd.Length - Prefixmap.SuffixList[i].Length)
                            {
                                LyricSnd = LyricSnd.Substring(0, lastidx);
                            }
                        }
                    }
                    for (int i = 0; i < Prefixmap.PrefixList.Count; i++)
                    {
                        if (Prefixmap.PrefixList[i] != "")
                        {
                            int firstidx = LyricSnd.IndexOf(Prefixmap.PrefixList[i]);
                            if (firstidx==0)
                            {
                                LyricSnd = LyricSnd.Substring(Prefixmap.PrefixList[i].Length);
                            }
                        }
                    }
                }
                if (!_PhonemeMap.ContainsKey(LyricSnd))
                {
                    _PhonemeMap.Add(LyricSnd, LyricSnd);
                }
            }
        }

        public static void SerializeTo(SplitDictionary Object, string FilePath)
        {
            FileStream msObj = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            ICSharpCode.SharpZipLib.GZip.GZipOutputStream boz = new ICSharpCode.SharpZipLib.GZip.GZipOutputStream(msObj);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(VocalIndexObject));
            js.WriteObject(boz, Object);
            boz.Flush();
            msObj.Close();
        }
        public static SplitDictionary SerializeFrom(string FilePath)
        {
            SplitDictionary ret = null;
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
                        DataContractJsonSerializer deseralizer = new DataContractJsonSerializer(typeof(SplitDictionary));
                        SplitDictionary model = (SplitDictionary)(deseralizer.ReadObject(ms));
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
