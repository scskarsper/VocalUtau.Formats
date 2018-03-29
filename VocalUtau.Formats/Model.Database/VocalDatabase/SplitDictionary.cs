using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Text.RegularExpressions;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.Database.VocalDatabase
{
    [Serializable]
    [DataContract]
    public class SplitDictionary
    {
        Regex _RegPattern = null;
        public Regex RegPattern
        {
            get
            {
                if (_RegPattern == null)
                {
                    _RegPattern = new Regex(@"(PrevNote|CurrentNote|NextNote)\[([0-9,n,-]{0,255})\]", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.CultureInvariant | RegexOptions.IgnorePatternWhitespace | RegexOptions.Singleline);
                };
                return _RegPattern;
            }
            set { _RegPattern = value; }
        }
        Dictionary<string, string> _KeyValueCache = new Dictionary<string, string>();

        public Dictionary<string, string> KeyValueCache
        {
            get { if (_KeyValueCache == null)_KeyValueCache = new Dictionary<string, string>(); return _KeyValueCache; }
            set { _KeyValueCache = value; }
        }

        System.Data.DataTable _Calcer = new System.Data.DataTable();

        public System.Data.DataTable Calcer
        {
            get { if (_Calcer == null)_Calcer = new System.Data.DataTable(); return _Calcer; }
            set { _Calcer = value; }
        }

        public class SplitAtom
        {
            long _AtomLength = 0;
            
            public long AtomLength
            {
                get { return _AtomLength; }
                set { _AtomLength = value; }
            }

            bool _LengthIsPercent = false;
            
            public bool LengthIsPercent
            {
                get { return _LengthIsPercent; }
                set { _LengthIsPercent = value; }
            }

            string _PhonemeAtom = "";
            
            public string PhonemeAtom
            {
                get { return _PhonemeAtom; }
                set { _PhonemeAtom = value; }
            }
        }
        //string _FunctionString = "PrevNote[]|CurrentNote[,]|CurrentNote[,] NextNote[n-1,100]";
        /// <summary>
        /// 参数定义：
        /// 确定当前音符的音标
        /// 上一个音符:PrevNote[音素索引]
        /// 中间音符:CurrentNote[音素索引]
        /// 下一个音符:NextNote[音素索引]
        /// 当音符索引为0时，取整个音素
        /// 音符Atom之间使用|分割
        /// 音符Atom公式末尾加入$代表指定Atom固定长度，值可以为整形和百分数（??%)
        /// 举例：CurrentNote[]|CurrentNote[n] NextNote[1]$120
        /// 举例：CurrentNote[]|CurrentNote[n] NextNote[1]$5%
        /// </summary>
        string _FunctionString = "CurrentNote[]|CurrentNote[n] NextNote[1]$120";
        //(PrevNote|CurrentNote|NextNote)\[[0-9,n,-]{0,255}\]
        [DataMember]
        public string FunctionString
        {
            get { return _FunctionString; }
            set { _FunctionString = value; }
        }

        Dictionary<string, List<string>> _NoteAtomMap = new Dictionary<string, List<string>>();

        [DataMember]
        public Dictionary<string, List<string>> NoteAtomMap
        {
            get { return _NoteAtomMap; }
            set { _NoteAtomMap = value; }
        }

        private List<string> GetAtomItem(string Lyric)
        {
            List<string> GetRet = new List<string>();
            try
            {
                GetRet = _NoteAtomMap[Lyric];
            }
            catch { ;}
            return GetRet;
        }
        private string CalcAtomStr(string Function, List<string> Prev, List<string> Curr, List<string> Next)
        {
            string ResultStr = Function;
            if (RegPattern.IsMatch(Function))
            {
                MatchCollection mcs = RegPattern.Matches(Function);
                for (int i = 0; i < mcs.Count; i++)
                {
                    Match m = mcs[i];
                    string key = m.Groups[1].Value.ToLower();
                    string val = m.Groups[2].Value.Trim();
                    List<string> CurMap = new List<string>();
                    string KeyIn = Curr[0] + "[]";
                    switch (key)
                    {
                        case "currentnote": CurMap = Curr; KeyIn = CurMap.Count == 0 ? "" : Function.ToLower().Replace("currentnote", CurMap[0]); break;
                        case "prevnote": CurMap = Prev; KeyIn = CurMap.Count == 0 ? "" : Function.ToLower().Replace("prevnote", CurMap[0]); break;
                        case "nextnote": CurMap = Next; KeyIn = CurMap.Count == 0 ? "" : Function.ToLower().Replace("nextnote", CurMap[0]); break;
                        default: continue;
                    }
                    string KeyAtom = "";
                   // if (KeyIn == "") return "";
                    if (KeyIn != "")
                    {
                        if (KeyValueCache.ContainsKey(KeyIn))
                        {
                            KeyAtom = KeyValueCache[KeyIn];
                        }
                        else
                        {
                            int idx = 0;
                            if (val != "")
                            {
                                val = val.Replace("n", (CurMap.Count - 1).ToString());
                                try
                                {
                                    idx = (int)Calcer.Compute(val, "");
                                }
                                catch { ;}
                            }
                            if (idx < CurMap.Count)
                            {
                                KeyAtom = CurMap[idx];
                                try
                                {
                                    KeyValueCache.Add(KeyIn, KeyAtom);
                                }
                                catch { ;}
                            }
                        }
                    }
                    ResultStr = ResultStr.Replace(m.Groups[0].Value, KeyAtom);
                }
                return ResultStr;
            }
            else
            {
                return "";
            }
        }
        public List<SplitAtom> GetCurrentNoteAtom(string prev, string current, string next)
        {
            List<string> GetCurrent = GetAtomItem(current);
            if (GetCurrent.Count == 0)
            {
                SplitAtom sa = new SplitAtom();
                sa.PhonemeAtom = current;
                return new List<SplitAtom>() { sa };
            }
            List<string> GetPrev = GetAtomItem(prev);
            List<string> GetNext = GetAtomItem(next);
            List<SplitAtom> RetAtom = new List<SplitAtom>();
            string[] List = _FunctionString.Split('|');
            for (int i = 0; i < List.Length; i++)
            {
                string si = List[i];
                int sIdx=si.LastIndexOf('$');
                string AtomBody = sIdx>0?si.Substring(0,sIdx):si;
                string AtomLength = "0";
                if (sIdx > 0 && sIdx+1 < si.Length)
                {
                    AtomLength = si.Substring(sIdx + 1);
                }

                string AtomStr = CalcAtomStr(AtomBody, GetPrev, GetCurrent,GetNext).Trim();
                if (AtomStr.Trim() != "")
                {
                    SplitAtom sa = new SplitAtom();
                    sa.PhonemeAtom = AtomStr;
                    sa.AtomLength=(long)Microsoft.VisualBasic.Conversion.Val(AtomLength);
                    if(AtomLength.IndexOf('%')!=-1)
                    {
                        sa.LengthIsPercent = true;
                    }
                    RetAtom.Add(sa);
                }
            }
            return RetAtom;
        }

        public static void SerializeTo(SplitDictionary Object, string FilePath)
        {
            FileStream msObj = new FileStream(FilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite);
            DataContractJsonSerializer js = new DataContractJsonSerializer(typeof(SplitDictionary));
            js.WriteObject(msObj, Object);
            msObj.Close();
        }
        public static SplitDictionary SerializeFrom(string FilePath)
        {
            SplitDictionary ret = null;
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

        public void MapSndList(List<SoundAtom> SndList, PrefixmapAtom Prefixmap)
        {
            foreach (SoundAtom snd in SndList)
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
                            if (firstidx == 0)
                            {
                                LyricSnd = LyricSnd.Substring(Prefixmap.PrefixList[i].Length);
                            }
                        }
                    }
                }
                string defs = LyricSnd;
                if (!_NoteAtomMap.ContainsKey(defs))
                {
                    _NoteAtomMap.Add(defs, new List<string>() { defs , "", "" });
                }
            }
        }



        public void SetupCurrentPhonmem(NoteObject prevObject, NoteObject curObject, NoteObject nextObject)
        {
            if (curObject == null) return;
            if (curObject.LockPhoneme) return;
            string prevLyric = "{R}";
            string nextLyric = "{R}";
            string currentLyric = curObject.Lyric;
            if (prevObject != null)
            {
                if (Math.Abs(curObject.Tick - (prevObject.Tick + prevObject.Length)) < 480)
                {
                    prevLyric = prevObject.Lyric;
                }
            }
            if (nextObject != null)
            {
                if (Math.Abs(nextObject.Tick - (curObject.Tick + curObject.Length)) < 480)
                {
                    nextLyric = nextObject.Lyric;
                }
            }
            List<SplitAtom> SList=GetCurrentNoteAtom(prevLyric, currentLyric, nextLyric);
            if (curObject.PhonemeAtoms.Count == SList.Count)
            {
                bool isSame = true;
                for (int i = 0; i < SList.Count; i++)
                {
                    if (curObject.PhonemeAtoms[i].PhonemeAtom != SList[i].PhonemeAtom)
                    {
                        isSame = false;
                        break;
                    }
                }
                if (!isSame)
                {
                    curObject.PhonemeAtoms.Clear();
                    for (int i = 0; i < SList.Count; i++)
                    {
                        string lastPn = "";
                        if (curObject.PhonemeAtoms.Count > 0) lastPn = curObject.PhonemeAtoms[curObject.PhonemeAtoms.Count - 1].PhonemeAtom;
                        if (lastPn != SList[i].PhonemeAtom)
                        {
                            NoteAtomObject nao = new NoteAtomObject();
                            nao.AtomLength = SList[i].AtomLength;
                            nao.LengthIsPercent = SList[i].LengthIsPercent;
                            nao.PhonemeAtom = SList[i].PhonemeAtom;
                            curObject.PhonemeAtoms.Add(nao);
                        }
                    }
                }
            }
            else
            {
                curObject.PhonemeAtoms.Clear();
                for (int i = 0; i < SList.Count; i++)
                {
                    string lastPn = "";
                    if (curObject.PhonemeAtoms.Count > 0) lastPn = curObject.PhonemeAtoms[curObject.PhonemeAtoms.Count - 1].PhonemeAtom;
                    if (lastPn != SList[i].PhonemeAtom)
                    {
                        NoteAtomObject nao = new NoteAtomObject();
                        nao.AtomLength = SList[i].AtomLength;
                        nao.LengthIsPercent = SList[i].LengthIsPercent;
                        nao.PhonemeAtom = SList[i].PhonemeAtom;
                        curObject.PhonemeAtoms.Add(nao);
                    }
                }
            }
        }

        public void UpdateLyrics(ref PartsObject parts, NoteObject curObj)
        {
            int curIndex = parts.NoteList.IndexOf(curObj);
            UpdateLyrics(ref parts, curIndex);
        }
        public void UpdateLyrics(ref PartsObject parts, int NoteIndex)
        {
            int curIndex = NoteIndex;
            NoteObject[] NoteMap = new NoteObject[] { null, null, null, null, null };

            NoteMap[0] = curIndex - 2 >= 0 ? parts.NoteList[curIndex - 2] : null;
            NoteMap[1] = curIndex - 1 >= 0 ? parts.NoteList[curIndex - 1] : null;
            NoteMap[2] = parts.NoteList[curIndex];
            NoteMap[3] = curIndex + 1 < parts.NoteList.Count ? parts.NoteList[curIndex + 1] : null;
            NoteMap[4] = curIndex + 2 < parts.NoteList.Count ? parts.NoteList[curIndex + 2] : null;
            // p2 p1 cur
            // p1 cur n1
            // cur n1 n2
            if (NoteMap[1] != null) SetupCurrentPhonmem(NoteMap[0], NoteMap[1], NoteMap[2]);
            if (NoteMap[2] != null) SetupCurrentPhonmem(NoteMap[1], NoteMap[2], NoteMap[3]);
            if (NoteMap[3] != null) SetupCurrentPhonmem(NoteMap[2], NoteMap[3], NoteMap[4]);
        }

        public void UpdateLyrics(ref PartsObject parts, int StartNoteIndex,int EndNoteIndex)
        {
            if (EndNoteIndex < 0) EndNoteIndex = parts.NoteList.Count - 1;
            if (StartNoteIndex < 0) StartNoteIndex = 0;
            if (EndNoteIndex > parts.NoteList.Count - 1) EndNoteIndex = parts.NoteList.Count - 1;
            if (StartNoteIndex == EndNoteIndex)
            {
                UpdateLyrics(ref parts, StartNoteIndex);
                return;
            }

            Dictionary<int, NoteObject> NoteMap = new Dictionary<int, NoteObject>();
            NoteMap.Add(StartNoteIndex - 2, StartNoteIndex - 2 >= 0 ? parts.NoteList[StartNoteIndex - 2] : null);
            NoteMap.Add(StartNoteIndex - 1, StartNoteIndex - 1 >= 0 ? parts.NoteList[StartNoteIndex - 1] : null);
            NoteMap.Add(EndNoteIndex + 2, EndNoteIndex + 2 < parts.NoteList.Count ? parts.NoteList[EndNoteIndex + 2] : null);
            NoteMap.Add(EndNoteIndex + 1, EndNoteIndex + 1 < parts.NoteList.Count ? parts.NoteList[EndNoteIndex + 1] : null);
            int cnt = 0;
            for (int i = StartNoteIndex; i <= EndNoteIndex; i++)
            {
                if (!NoteMap.ContainsKey(i))
                {
                    if (i < parts.NoteList.Count) { NoteMap.Add(i, parts.NoteList[i]); cnt++; }
                }
            }
            for (int i = StartNoteIndex; i < EndNoteIndex; i++)
            {
                SetupCurrentPhonmem(NoteMap[i - 1], NoteMap[i], NoteMap[i + 1]);
            }
        }

        public void UpdateLyrics_Aysnc(AsyncWorkCallbackHandler CallBack, ref PartsObject parts, int NoteStartIndex = 0, int NoteEndIndex = -1)
        {
            PartsObject tParts = parts;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                UpdateLyrics(ref tParts, NoteStartIndex, NoteEndIndex);
                if (CallBack != null) CallBack(tParts,NoteStartIndex, NoteEndIndex);
            });
        }
        public delegate void AsyncWorkCallbackHandler(PartsObject parts,int NoteStartIndex, int NoteEndIndex);

        public void UpdateOutboundsLyric(ref PartsObject parts, int StartNoteIndex, int EndNoteIndex)
        {
            if (EndNoteIndex < 0) EndNoteIndex = parts.NoteList.Count - 1;
            if (StartNoteIndex < 0) StartNoteIndex = 0;
            if (EndNoteIndex > parts.NoteList.Count - 1) EndNoteIndex = parts.NoteList.Count - 1;

            NoteObject[] NoteMap = new NoteObject[] { null, null, null, null, null,null };

            NoteMap[0] = StartNoteIndex - 2 >= 0 ? parts.NoteList[StartNoteIndex - 2] : null;
            NoteMap[1] = StartNoteIndex - 1 >= 0 ? parts.NoteList[StartNoteIndex - 1] : null;
            NoteMap[2] = parts.NoteList[StartNoteIndex];


            NoteMap[3] = parts.NoteList[EndNoteIndex];
            NoteMap[4] = EndNoteIndex + 1 < parts.NoteList.Count ? parts.NoteList[EndNoteIndex + 1] : null;
            NoteMap[5] = EndNoteIndex + 2 < parts.NoteList.Count ? parts.NoteList[EndNoteIndex + 2] : null;

            if (NoteMap[1] != null) SetupCurrentPhonmem(NoteMap[0], NoteMap[1], NoteMap[2]);
            if (NoteMap[4] != null) SetupCurrentPhonmem(NoteMap[3], NoteMap[4], NoteMap[5]);
        }
        public void UpdateOutboundsLyric_Aysnc(AsyncWorkCallbackHandler CallBack, ref PartsObject parts, int NoteStartIndex = 0, int NoteEndIndex = -1)
        {
            PartsObject tParts = parts;
            System.Threading.Tasks.Task.Factory.StartNew(() =>
            {
                UpdateLyrics(ref tParts, NoteStartIndex, NoteEndIndex);
                if (CallBack != null) CallBack(tParts, NoteStartIndex, NoteEndIndex);
            });
        }
        /*
        

        private void FillPhoneme(List<SplitAtom> salist, ref NoteObject obj)
        {
            if (salist != null && obj.Length > 32)
            {
                if (!obj.LockPhoneme)
                {
                    for (int i = 0; i < salist.Count; i++)
                    {
                        SplitAtom sa1 = salist[i];
                        if (i < obj.PhonemeAtoms.Count)
                        {
                            if (obj.PhonemeAtoms[i].PhonemeAtom != sa1.PhonemeAtom)
                            {
                                NoteAtomObject nao = new NoteAtomObject();
                                nao.AtomLength = sa1.AtomLength;
                                nao.LengthIsPercent = sa1.LengthIsPercent;
                                nao.PhonemeAtom = sa1.PhonemeAtom;
                                obj.PhonemeAtoms[i] = nao;
                            }
                            else
                            {
                                obj.PhonemeAtoms[i].LengthIsPercent = sa1.LengthIsPercent;
                                obj.PhonemeAtoms[i].AtomLength = sa1.AtomLength;
                            }
                        }
                        else
                        {
                            NoteAtomObject nao = new NoteAtomObject();
                            nao.AtomLength = sa1.AtomLength;
                            nao.LengthIsPercent = sa1.LengthIsPercent;
                            nao.PhonemeAtom = sa1.PhonemeAtom;
                            obj.PhonemeAtoms.Add(nao);
                        }
                    }
                    while (obj.PhonemeAtoms.Count > salist.Count)
                    {
                        obj.PhonemeAtoms.RemoveAt(obj.PhonemeAtoms.Count - 1);
                    }
                }
            }
        }
        private void SetPhonemes(SplitMap smap, ref NoteObject preObj, ref NoteObject nxtObj)
        {
            FillPhoneme(smap.PreAtom, ref preObj);
            FillPhoneme(smap.NxtAtom, ref nxtObj);
        }
        public void SetupLyrics(ref NoteObject preObj, ref NoteObject nxtObj)
        {
            NoteObject n1 = preObj;
            NoteObject n2 = nxtObj;
            if (n1 == null) n1 = new NoteObject(long.MaxValue, long.MaxValue, 60, "R");
            if (n2 != null)
            {
                if (Math.Abs(n2.Tick - (n1.Tick + n1.Length)) > 480)
                {
                    n2 = new NoteObject(long.MaxValue, long.MaxValue, 60, "R");
                }
            }
            if (n2 == null) n2 = new NoteObject(long.MaxValue, long.MaxValue, 60, "R");
            string n1Lyric = n1.Lyric;
            string n2Lyric = n2.Lyric;
            string key1 = n1Lyric + "," + n2Lyric;
            if (key1 == "R,R") return;
            if (PhonemeMap.ContainsKey(key1))
            {
                SetPhonemes(PhonemeMap[key1], ref n1, ref n2);
            }
            else
            {
                if (n1Lyric == "R") return;
                string key2 = n1Lyric + ",";
                if (PhonemeMap.ContainsKey(key2))
                {
                    SetPhonemes(PhonemeMap[key2], ref n1, ref n2);
                }
            }
        }
        public void SetupLyrics(ref PartsObject parts, NoteObject curObj)
        {
            int curIndex = parts.NoteList.IndexOf(curObj);

            NoteObject a1 = null;
            NoteObject a2 = null;
            NoteObject a3 = null;

            a2 = parts.NoteList[curIndex];
            if (curIndex > 0) a1 = parts.NoteList[curIndex - 1];
            if (curIndex < parts.NoteList.Count - 1) a3 = parts.NoteList[curIndex + 1];

            SetupLyrics(ref a1, ref a2);
            SetupLyrics(ref a2, ref a3);
        }
        public void SetupLyrics(ref PartsObject parts)
        {
            parts.NoteList.Sort();
            for (int i = 0; i < parts.NoteList.Count; i++)
            {
                int n2 = i + 1;
                if (n2 >= parts.NoteList.Count)
                {
                    //Last
                    NoteObject o1 = parts.NoteList[i];
                    NoteObject o2 = null;
                    SetupLyrics(ref o1, ref o2);
                }
                else
                {
                    NoteObject o1 = parts.NoteList[i];
                    NoteObject o2 = parts.NoteList[i + 1];
                    SetupLyrics(ref o1, ref o2);
                }
            }
        }*/
    }
}
