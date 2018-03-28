using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VocalUtau.Formats.Model.Database.VocalDatabase;

namespace VocalUtau.Formats.Model.USTs.Otos
{
    public class Presamp2DictSerializer
    {
        static void LoadSamp(string[] T, ref SplitDictionary Dict)
        {
            bool isVOWEL = false;
            bool isCONSONANT = false;
            bool isENDType = false;

            string EndCH = "R";
            string[] S = T;
            foreach (string s in S)
            {
                if (s.Substring(0, 1) == "[")
                {
                    isVOWEL = false;
                    isCONSONANT = false;
                }
                if (s.IndexOf("[VOWEL]") >= 0)
                {
                    isVOWEL = true;
                    continue;
                }
                if (s.IndexOf("[CONSONANT]") >= 0)
                {
                    isCONSONANT = true;
                    continue;
                }
                if (s.IndexOf("[ENDTYPE]") >= 0)
                {
                    isENDType = true;
                    continue;
                }
                if (isENDType)
                {
                    if (s.IndexOf("%v% ") != -1)
                    {
                        string[] w = s.Split(new string[]{"%v% "},StringSplitOptions.None);
                        try
                        {
                            EndCH = w[1];
                        }
                        catch { ;}
                    }
                }
                if (isVOWEL)
                {
                    string[] k = s.Split('=');
                    string Si1 = k[0];
                    string Si2 = k[1];
                    string Si3 = k[2];
                    string[] o = Si3.Split(',');
                    foreach (string so in o)
                    {
                        try
                        {
                            if (Dict.NoteAtomMap.ContainsKey(so))
                            {
                                Dict.NoteAtomMap[so][2]=Si1;
                            }
                            else
                            {
                                List<string> lt = new List<string>() { so, "", Si1 };
                                Dict.NoteAtomMap.Add(so, lt);
                            }
                        }
                        catch { ;}
                    }
                }
                if (isCONSONANT)
                {
                    string[] k = s.Split('=');
                    string Si1 = k[0];
                    string Si2 = k[1];
                    string[] o = Si2.Split(',');
                    foreach (string so in o)
                    {
                        try
                        {
                            if (Dict.NoteAtomMap.ContainsKey(so))
                            {
                                Dict.NoteAtomMap[so][1] = Si1;
                            }
                            else
                            {
                                List<string> lt = new List<string>() { so, Si1, "" };
                                Dict.NoteAtomMap.Add(so, lt);
                            }
                        }
                        catch { ;}
                    }
                }
            }
            try
            {
                if (Dict.NoteAtomMap.ContainsKey("{R}"))
                {
                    Dict.NoteAtomMap["{R}"][1] = EndCH;
                }
                else
                {
                    List<string> lt = new List<string>() { "", EndCH, "" };
                    Dict.NoteAtomMap.Add("{R}", lt);
                }
            }
            catch { ;}
        }
        public static SplitDictionary DeSerialize(string FilePath)
        {
            SplitDictionary ret = new SplitDictionary();

            Dictionary<string, string> VMap = new Dictionary<string, string>();
            Dictionary<string, string> CMap = new Dictionary<string, string>();

            Encoding FileEnc = Encoding.ASCII;// FileEncodingUtils.GetEncodingJIS(FilePath);

            string[] Datas = System.IO.File.ReadAllLines(FilePath, FileEnc);
            LoadSamp(Datas, ref ret);

            return ret;

        }
    }
}
