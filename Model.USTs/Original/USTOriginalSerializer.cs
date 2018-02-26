using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using VocalUtau.Formats.Model.VocalObject;

namespace VocalUtau.Formats.Model.USTs.Original
{
    public class USTOriginalSerializer
    {
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(
            string lpAppName, // points to section name
            string lpKeyName, // points to key name
            string lpDefault, // points to default string
            byte[] lpReturnedString, // points to destination buffer
            uint nSize, // size of destination buffer
            string lpFileName  // points to initialization filename
        );
        [DllImport("kernel32")]
        private static extern uint GetPrivateProfileString(
            string lpAppName, // points to section name
            string lpKeyName, // points to key name
            string lpDefault, // points to default string
            StringBuilder lpReturnedString, // points to destination buffer
            uint nSize, // size of destination buffer
            string lpFileName  // points to initialization filename
        );


        /// <summary>
        /// 读取section
        /// </summary>
        /// <param name="Strings"></param>
        /// <returns></returns>

        private static List<string> ReadSections(string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint len = GetPrivateProfileString(null, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < len; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        public static string IniReadValue(string Section, string Key,string Default, string iniFilename)
        {
            StringBuilder temp = new StringBuilder(255); 
            uint i = GetPrivateProfileString(Section, Key, Default, temp, (uint)255, iniFilename);
            return temp.ToString();
        }
        public static double IniReadDouble(string Section, string Key, double Default, string iniFilename)
        {
            double t = Default;
            double.TryParse(IniReadValue(Section, Key, Default.ToString(), iniFilename), out t);
            return t;
        }
        /// <summary>
        /// 读取指定区域Keys列表。
        /// </summary>
        /// <param name="Section"></param>
        /// <param name="Strings"></param>
        /// <returns></returns>

        public List<string> ReadSingleSection(string Section, string iniFilename)
        {
            List<string> result = new List<string>();
            byte[] buf = new byte[65536];
            uint lenf = GetPrivateProfileString(Section, null, null, buf, (uint)buf.Length, iniFilename);
            int j = 0;
            for (int i = 0; i < lenf; i++)
                if (buf[i] == 0)
                {
                    result.Add(Encoding.Default.GetString(buf, j, i - j));
                    j = i + 1;
                }
            return result;
        }
        public static USTOriginalProject Deserialize(string ustfile)
        {
            USTOriginalProject Original = new USTOriginalProject();
            List<string> sectionList = ReadSections(ustfile);
            for (int i = 0; i < sectionList.Count; i++)
            {
                int Cdx=0;
                string sectionName = sectionList[i];
                if (sectionName.ToUpper() == "#SETTING")
                {
                    Original.Tempo = IniReadDouble(sectionName, "Tempo", 120.0, ustfile);
                    Original.Tracks = (int)IniReadDouble(sectionName, "Tracks", 1, ustfile);
                    Original.ProjectName = IniReadValue(sectionName, "ProjectName", "", ustfile);
                    Original.VoiceDir = IniReadValue(sectionName, "VoiceDir", "", ustfile);
                    Original.OutFile = IniReadValue(sectionName, "OutFile", "", ustfile);
                    Original.CacheDir = IniReadValue(sectionName, "CacheDir", "", ustfile);
                    Original.Tool1= IniReadValue(sectionName, "Tool1", "wavtool.exe", ustfile);
                    Original.Tool2 = IniReadValue(sectionName, "Tool2", "resampler.exe", ustfile);
                    Original.Flags = IniReadValue(sectionName, "Flags", "", ustfile);
                    bool m2 = false;
                    bool.TryParse(IniReadValue(sectionName, "Mode2", "false", ustfile), out m2);
                }
                else if (sectionName[0] == '#' && int.TryParse(sectionName.Substring(1), out Cdx))
                {
                    USTOriginalNote UNote = new USTOriginalNote();
                    UNote.Length = (long)IniReadDouble(sectionName, "Length", 0, ustfile);
                    UNote.Lyric = IniReadValue(sectionName, "Lyric", "", ustfile);
                    UNote.NoteNum = (int)IniReadDouble(sectionName, "NoteNum", 60, ustfile);
                    UNote.PreUtterance = (int)IniReadDouble(sectionName, "PreUtterance", 0, ustfile);
                    UNote.Overlap = (int)IniReadDouble(sectionName, "VoiceOverlap", 0, ustfile);
                    UNote.Intensity = (int)IniReadDouble(sectionName, "Intensity", 0, ustfile);
                    UNote.Modulation = (int)IniReadDouble(sectionName, "Modulation", 0, ustfile);
                    UNote.Tempo = (long)IniReadDouble(sectionName, "Tempo", 0, ustfile);
                    UNote.StartPoint = (int)IniReadDouble(sectionName, "StartPoint", 0, ustfile);
                    UNote.Velocity = (int)IniReadDouble(sectionName, "Velocity", 0, ustfile);
                    UNote.Flags = IniReadValue(sectionName, "Flags", "", ustfile);
                    UNote.Envelope = IniReadValue(sectionName, "Envelope", "", ustfile); 
                    UNote.Apreuttr = (int)IniReadDouble(sectionName, "@preuttr", 0, ustfile);
                    UNote.Aoverlap = (int)IniReadDouble(sectionName, "@overlap", 0, ustfile);
                    UNote.Astpoint = (int)IniReadDouble(sectionName, "@stpoint", 0, ustfile);
                    UNote.PBType = (int)IniReadDouble(sectionName, "PBType", 5, ustfile);
                    UNote.PitchBend = IniReadValue(sectionName, "PitchBend", "", ustfile);
                    UNote.PBStart = (int)IniReadDouble(sectionName, "PBStart", 0, ustfile);
                    UNote.VBR = IniReadValue(sectionName, "VBR", "", ustfile);
                    UNote.PBW = IniReadValue(sectionName, "PBW", "", ustfile);
                    UNote.PBY = IniReadValue(sectionName, "PBY", "", ustfile);
                    UNote.PBS = IniReadValue(sectionName, "PBS", "", ustfile);
                    Original.Notes.Add(Cdx,UNote);
                    Cdx = 0;
                }
            }
            return Original;
        }

        public static PartsObject UST2Parts(USTOriginalProject ust)
        {
            PartsObject po = new PartsObject(ust.ProjectName);
            po.Tempo = ust.Tempo;
            long TotalTick = 0;
//            po.TickLength = 0;
            po.PartResampler = ust.Tool2;
            po.Flags = ust.Flags;
            for (int i = 0; i < ust.Notes.Count; i++)
            {
                long stt = TotalTick;
                long len = ust.Notes[i].Length;
                if (ust.Notes[i].Lyric != "R")
                {
                    NoteObject no = new NoteObject(stt, len, ust.Notes[i].NoteNum);
                    no.Lyric = ust.Notes[i].Lyric;
                    no.PhonemeAtoms[0].Flags = ust.Notes[i].Flags;
                    no.PhonemeAtoms[0].Intensity = ust.Notes[i].Intensity;
                    no.PhonemeAtoms[0].Modulation = ust.Notes[i].Modulation;
                    no.PhonemeAtoms[0].Overlap = ust.Notes[i].Overlap;
                    no.PhonemeAtoms[0].PreUtterance = ust.Notes[i].PreUtterance;
                    no.PhonemeAtoms[0].StartPoint = ust.Notes[i].StartPoint;
                    no.PhonemeAtoms[0].Velocity = ust.Notes[i].Velocity;
                    no.PhonemeAtoms[0].Envelopes.AddRange(ust.Notes[i].EnvelopAnalyse());
                    po.NoteList.Add(no);
                    TotalTick += ust.Notes[i].Length;// -ust.Notes[i].Overlap;
                }
            }
            po.TickLength = TotalTick;
            return po;
        }
    }
}
