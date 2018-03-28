using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;
using VocalUtau.Formats.Model.Utils;

namespace VocalUtau.Formats.Model.USTs.Otos
{
    public class PrefixMapSerialzier
    {
        private static List<string> KeyChar = new List<string>() { "C", "C#", "D", "D#", "E", "F", "F#", "G", "G#", "A", "A#", "B" };
        public static PrefixmapAtom DeSerialize(string FilePath)
        {
            PrefixmapAtom pfa = new PrefixmapAtom();
            if (!System.IO.File.Exists(FilePath)) return pfa;
            Encoding FileEnc = FileEncodingUtils.GetEncodingJIS(FilePath);
            string[] Datas = System.IO.File.ReadAllLines(FilePath, FileEnc);
            for (int i = 0; i < Datas.Length; i++)
            {
                string[] pfl = Datas[i].Split('\t');
                string NoteS = pfl[0];
                string pref = pfl[1];
                string sfx = pfl[2];
                uint NoteNum = getNoteNumber(NoteS);
                if (!pfa.PrefixList.Contains(pref))
                {
                    pfa.PrefixList.Add(pref);
                }
                if (!pfa.SuffixList.Contains(sfx))
                {
                    pfa.SuffixList.Add(sfx);
                }
                if (pfa.PreFix.ContainsKey(NoteNum))
                {
                    pfa.PreFix[NoteNum] = pref;
                }
                else
                {
                    pfa.PreFix.Add(NoteNum, pref);
                }
                if (pfa.SufFix.ContainsKey(NoteNum))
                {
                    pfa.SufFix[NoteNum] = sfx;
                }
                else
                {
                    pfa.SufFix.Add(NoteNum, sfx);
                }
            }
            return pfa;
        }
        private static uint getNoteNumber(string UtauKeyStr)
        {
            uint ret = 0;
            UtauKeyStr = UtauKeyStr.ToUpper();
            char c1 = UtauKeyStr[0];
            char c2 = UtauKeyStr[1];
            string K = "";
            string O = "";
            if (c1 >= 'A' && c1 <= 'G')
            {
                if (c2 == '#')
                {
                    K = UtauKeyStr.Substring(0, 2);
                    O = UtauKeyStr.Substring(2);
                }
                else
                {
                    K = UtauKeyStr.Substring(0, 1);
                    O = UtauKeyStr.Substring(1);
                }
            }
            int OS = int.Parse(O) - 1;
            int KS = KeyChar.IndexOf(K);
            ret = (uint)(12 * OS + KS + 24);
            return ret;
        }
    }
}
